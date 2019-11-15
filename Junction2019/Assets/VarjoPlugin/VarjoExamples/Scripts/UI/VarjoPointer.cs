// Copyright 2019 Varjo Technologies Oy. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Varjo;

/// <summary>
/// Visual representation for ui pointing.
/// Works with both canvas and geometry.
/// Changes input automatically to last used or connected method.
/// </summary>

namespace VarjoExample
{
    public class VarjoPointer : MonoBehaviour
    {
        [Header("Input module for canvas ui hits")]
        public VarjoInputModule varjoInputModule;

        [Header("Primary hand controller")]
        public Transform hand;
        public Vector3 handRotationOffset = new Vector3(60.0f, 0.0f, 0.0f);

        [Header("Object to show where user is aiming")]
        public GameObject crosshair;

        [Header("Object to show when aiming with hand contoller")]
        public GameObject laserPointer;

        [Header("Line to render from hand to crosshair (optional)")]
        public LineRenderer laserPointerLine;

        [Header("How far crosshair should hover from hit position")]
        public float distanceFromHit = 0.001f;

        Vector3 headPosition;
        Quaternion headRotation;
        bool worldHitSuccess;
        RaycastHit worldHit;

        void Awake()
        {
            if (varjoInputModule == null)
            {
                Debug.LogError("Varjo pointer is missing input module reference. Disabling pointer.", gameObject);
                enabled = false;
                return;
            }

            if (crosshair == null)
            {
                Debug.LogWarning("Varjo pointer is missing crosshair.", gameObject);
            }

            if (laserPointer == null)
            {
                Debug.LogWarning("Varjo pointer is missing laser pointer.", gameObject);
            }
        }

        void Update()
        {
            // Use the selection method that was called last time
            if (varjoInputModule.LastInputMethod == VarjoInputModule.InputMethod.CONTROLLER && hand.gameObject.activeInHierarchy)
            {
                if (laserPointer != null && !laserPointer.activeSelf)
                {
                    laserPointer.SetActive(true);
                }

                transform.position = hand.position;
                transform.rotation = hand.rotation;
                transform.Rotate(handRotationOffset, Space.Self);
            }
            else
            {
                if (laserPointer != null && laserPointer.activeSelf)
                {
                    laserPointer.SetActive(false);
                }

                if (VarjoManager.Instance.HeadTransform != null)
                {
                    transform.position = VarjoManager.Instance.HeadTransform.position;
                    transform.rotation = VarjoManager.Instance.HeadTransform.rotation;
                }
            }

            if (crosshair != null)
            {
                bool showCrosshair = true;

                // Show crosshair on canvas if possible
                if (varjoInputModule.PointerHitCanvas)
                {
                    crosshair.transform.position = varjoInputModule.PointerWorldPosition + varjoInputModule.PointerWorldPositionNormal * distanceFromHit;
                    crosshair.transform.forward = varjoInputModule.PointerWorldPositionNormal;
                }
                // if not, show it on world geometry
                else
                {
                    if (Physics.Raycast(transform.position, transform.forward, out worldHit))
                    {
                        crosshair.transform.position = worldHit.point + worldHit.normal * distanceFromHit;
                        crosshair.transform.forward = worldHit.normal;
                    }
                    else
                    {
                        // Hide crosshair when there is nothing to place it on
                        showCrosshair = false;
                    }
                }

                // Update crosshair visibility if needed
                if (crosshair.activeSelf != showCrosshair)
                {
                    crosshair.SetActive(showCrosshair);
                }

                // Stretch laser pointer line from hand to crosshair
                if (laserPointerLine != null && laserPointerLine.gameObject.activeInHierarchy)
                {
                    for (int i = 0; i < laserPointerLine.positionCount; ++i)
                    {
                        laserPointerLine.SetPosition(i, hand.position + ((crosshair.transform.position - hand.position) * ((float)i / (float)laserPointerLine.positionCount)));
                    }
                }
            }
        }
    }
}
