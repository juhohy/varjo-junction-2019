// Copyright 2019 Varjo Technologies Oy. All rights reserved.

using System.Collections.Generic;
using UnityEngine;
using Varjo;

namespace VarjoExample
{
    /// <summary>
    /// Basic teleport script by holding touchpad in the controller
    /// </summary>
    public class VarjoTeleport : MonoBehaviour
    {
        // Public variables to customize teleport, set in inspector
        [Header("Variables")]
        public Color validTeleportColor = Color.cyan;
        public Color invalidTeleportColor = Color.red;

        public LayerMask teleportableLayers;        // Collision layers for teleport arc
        public float maxTargetSurfaceAngle = 10f;   // Prevent teleporting in walls

        public float velocity = 5f;                 // Velocity for simulated trajectory
        public float gravity = 25f;                 // Gravity for simulated trajectory
        public float controllerPitchFactor = 1f;    // How much controller angle affects the velocity
        public int pointsOnArc = 30;                // Affects performance and visual quality

        // Public scene references, set in inspector
        [Header("References")]
        public Transform teleportPointer;           // Teleport pointer
        public Transform varjoUser;                 // What should we move when teleporting
        public Transform varjoCamera;               // Where is our head
        public GameObject teleportTargetPrefab;     // Visual representation of teleport target

        // Private references and variables
        Varjo_SteamVR_TrackedObject trackedObject;  // Current controller
        List<Vector3> arc = new List<Vector3>();    // List of points on the arc
        LineRenderer teleportArcRenderer;           // Arc renderer
        Transform teleportTarget;                   // Instantiated target visuals
        Renderer teleportTargetRenderer;            // Target renderer

        bool canTeleport;                           // Can we teleport to pointer target
        RaycastHit hit;
        Vector3 point;                              // Current point on arc
        Vector3 lastPoint;                          // Previous point on arc
        Vector3 hitPoint;                           // The point where arc hit a collider
        Vector3 forwardVelocity;                    // Calculated velocity for simulated trajectory
        Vector3 trackingSpaceGravity;               // Tracking space gravity for simulated trajectory
        Vector3 controllerPitchEffect;              // Calculated effect of controller pitch to simulated trajectory


        // Helper for getting input from SteamVR
        // You can use get input directly like this or register to SteamVR input events
        Varjo_SteamVR_Controller.Device Controller
        {
            get { return Varjo_SteamVR_Controller.Input((int)trackedObject.index); }
        }

        // Get required references, sanity check references, instansiate required visuals
        void Awake()
        {
            trackedObject = GetComponent<Varjo_SteamVR_TrackedObject>();

            if (varjoUser == null || varjoCamera == null || teleportTargetPrefab == null)
            {
                Debug.LogError("Varjo teleport disabled from " + gameObject + ": Missing references.", gameObject);
                enabled = false;
            }

            teleportArcRenderer = teleportPointer.GetComponent<LineRenderer>();
            teleportTarget = Instantiate(teleportTargetPrefab).transform;
            teleportTargetRenderer = teleportTarget.GetComponentInChildren<Renderer>();
        }

        void OnValidate()
        {
            if (pointsOnArc < 3)
            {
                pointsOnArc = 3;
                Debug.LogWarning("The arc should have at least three points.");
            }
        }

        // Shoot teleport arc when holding touchpad and teleport when released if possible
        void Update()
        {
            if (Controller.GetPress(Varjo_SteamVR_Controller.ButtonMask.Touchpad))
            {
                arc.Clear();
                arc.Add(teleportPointer.position);

                lastPoint = teleportPointer.position;
                forwardVelocity = teleportPointer.forward * velocity;
                trackingSpaceGravity = varjoUser.up * -gravity;
                controllerPitchEffect = Mathf.Max((Vector3.Dot(varjoUser.up, teleportPointer.up)), 0f) * controllerPitchFactor * forwardVelocity;

                for (int i = 1; i < pointsOnArc; i++)
                {
                    float t = i * 1f / pointsOnArc;
                    Vector3 point = GetPointFromArc(teleportPointer.position, forwardVelocity + controllerPitchEffect, trackingSpaceGravity, t);

                    if (Physics.Linecast(lastPoint, point, out hit, teleportableLayers))
                    {
                        arc.Add(hit.point);
                        hitPoint = hit.point;
                        canTeleport = AngleAllowed(hit.normal);
                        break;
                    }
                    else
                    {
                        arc.Add(point);
                        canTeleport = false;
                    }
                    lastPoint = point;
                }
                // Draw arc with color indicating if the current target is valid
                // Valid target is also indicated with a rectangle on the ground
                if (canTeleport)
                {
                    ShowArc(validTeleportColor);
                    ShowTarget();
                }
                else
                {
                    ShowArc(invalidTeleportColor);
                    teleportTarget.gameObject.SetActive(false);
                }
            }
            else
            {
                teleportPointer.gameObject.SetActive(false);
                teleportTarget.gameObject.SetActive(false);
            }

            if (Controller.GetPressUp(Varjo_SteamVR_Controller.ButtonMask.Touchpad) && canTeleport)
            {
                Teleport();
            }
        }

        void ShowArc(Color color)
        {
            teleportPointer.gameObject.SetActive(true);
            teleportArcRenderer.positionCount = arc.Count;
            teleportArcRenderer.SetPositions(arc.ToArray());
            teleportArcRenderer.material.SetColor("_EmissionColor", color);
        }

        void ShowTarget()
        {
            teleportTarget.gameObject.SetActive(true);
            teleportTarget.position = hitPoint;
            teleportTarget.rotation = Quaternion.identity;
            teleportTargetRenderer.material.SetColor("_EmissionColor", validTeleportColor);
        }

        void Teleport()
        {
            // Teleport relative to head position to avoid spatial confusion
            Vector3 userOffsetFromTrackingOrigin = varjoUser.position - varjoCamera.position;
            userOffsetFromTrackingOrigin.y = 0;
            varjoUser.position = hitPoint + userOffsetFromTrackingOrigin;

            teleportTarget.gameObject.SetActive(false);
            canTeleport = false;
        }

        // Checks if angle of the surface is smaller or equal to set maximum angle
        bool AngleAllowed(Vector3 angle)
        {
            return Vector3.Angle(Vector3.up, angle) <= maxTargetSurfaceAngle;
        }

        // Returns point from a simulated trajectory
        public Vector3 GetPointFromArc(Vector3 start, Vector3 velocity, Vector3 gravity, float time)
        {
            Vector3 point = new Vector3();
            point.x = start.x + velocity.x * time + 0.5f * gravity.x * time * time;
            point.y = start.y + velocity.y * time + 0.5f * gravity.y * time * time;
            point.z = start.z + velocity.z * time + 0.5f * gravity.z * time * time;
            return point;
        }
    }
}
