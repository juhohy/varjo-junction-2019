// Copyright 2019 Varjo Technologies Oy. All rights reserved.

using UnityEngine;
using Varjo;

namespace VarjoExample
{
    /// <summary>
    /// Pick up and throw objects entering this trigger volume when holding controller trigger.
    /// </summary>
    [RequireComponent(typeof(Varjo_SteamVR_TrackedObject))]
    [RequireComponent(typeof(FixedJoint))]
    public class VarjoPickup : MonoBehaviour
    {
        // Pickup hand
        Varjo_SteamVR_TrackedObject trackedObject;

        // What are we contact with
        Rigidbody collidingRigidbody;

        // What are we holding
        Rigidbody heldRigidbody;

        // Joint to keep hold object in hand
        FixedJoint fixedJoint;

        // Helper for getting input from SteamVR
        // You can use get input directly like this or register to SteamVR input events
        Varjo_SteamVR_Controller.Device Controller
        {
            get { return Varjo_SteamVR_Controller.Input((int)trackedObject.index); }
        }

        // Get components required for by this script
        void Awake()
        {
            trackedObject = GetComponent<Varjo_SteamVR_TrackedObject>();
            fixedJoint = GetComponent<FixedJoint>();
        }

        // Keep track of object entering and exiting this trigger
        void OnTriggerEnter(Collider other)
        {
            SetCollidingRigidbody(other);
        }

        void OnTriggerStay(Collider other)
        {
            SetCollidingRigidbody(other);
        }

        void OnTriggerExit(Collider other)
        {
            if (!collidingRigidbody)
                return;

            collidingRigidbody = null;
        }

        void SetCollidingRigidbody(Collider col)
        {
            if (collidingRigidbody || !col.GetComponent<Rigidbody>())
                return;

            collidingRigidbody = col.gameObject.GetComponent<Rigidbody>();
            Controller.TriggerHapticPulse(1000);
        }

        // Get trigger input from controller and call pickup and throw based on that
        void Update()
        {
            if (Controller.GetHairTriggerDown() && collidingRigidbody)
            {
                Pickup();
            }

            if (Controller.GetHairTriggerUp() && heldRigidbody)
            {
                Throw();
            }
        }

        // When picking up a new object, we connect it to hand with a joing
        void Pickup()
        {
            heldRigidbody = collidingRigidbody.GetComponent<Rigidbody>();
            if (heldRigidbody == null)
            {
                Debug.LogError("No Rigidbody component in heldObject. Can't pick it up.");
                return;
            }
            collidingRigidbody = null;
            fixedJoint.connectedBody = heldRigidbody;
        }

        // When throwing object we release the joint and apply current controller velocity to released object
        void Throw()
        {
            fixedJoint.connectedBody = null;
            heldRigidbody.velocity = Controller.velocity;
            heldRigidbody.angularVelocity = Controller.angularVelocity;
            heldRigidbody = null;
        }
    }
}
