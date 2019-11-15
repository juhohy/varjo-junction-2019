//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Test SteamVR_Controller support.
//
//=============================================================================

// Modified By Varjo

using UnityEngine;
using System.Collections.Generic;
using Varjo.Valve.VR;

namespace Varjo
{
    public class Varjo_SteamVR_TestController : MonoBehaviour
    {
        List<int> controllerIndices = new List<int>();

        private void OnDeviceConnected(int index, bool connected)
        {
            var system = OpenVR.System;
            if (system == null || system.GetTrackedDeviceClass((uint)index) != ETrackedDeviceClass.Controller)
                return;

            if (connected)
            {
                Debug.Log(string.Format("Controller {0} connected.", index));
                PrintControllerStatus(index);
                controllerIndices.Add(index);
            }
            else
            {
                Debug.Log(string.Format("Controller {0} disconnected.", index));
                PrintControllerStatus(index);
                controllerIndices.Remove(index);
            }
        }

        void OnEnable()
        {
            Varjo_SteamVR_Events.DeviceConnected.Listen(OnDeviceConnected);
        }

        void OnDisable()
        {
            Varjo_SteamVR_Events.DeviceConnected.Remove(OnDeviceConnected);
        }

        void PrintControllerStatus(int index)
        {
            var device = Varjo_SteamVR_Controller.Input(index);
            Debug.Log("index: " + device.index);
            Debug.Log("connected: " + device.connected);
            Debug.Log("hasTracking: " + device.hasTracking);
            Debug.Log("outOfRange: " + device.outOfRange);
            Debug.Log("calibrating: " + device.calibrating);
            Debug.Log("uninitialized: " + device.uninitialized);
            Debug.Log("pos: " + device.transform.pos);
            Debug.Log("rot: " + device.transform.rot.eulerAngles);
            Debug.Log("velocity: " + device.velocity);
            Debug.Log("angularVelocity: " + device.angularVelocity);

            var l = Varjo_SteamVR_Controller.GetDeviceIndex(Varjo_SteamVR_Controller.DeviceRelation.Leftmost);
            var r = Varjo_SteamVR_Controller.GetDeviceIndex(Varjo_SteamVR_Controller.DeviceRelation.Rightmost);
            Debug.Log((l == r) ? "first" : (l == index) ? "left" : "right");
        }

        EVRButtonId[] buttonIds = new EVRButtonId[] {
        EVRButtonId.k_EButton_ApplicationMenu,
        EVRButtonId.k_EButton_Grip,
        EVRButtonId.k_EButton_SteamVR_Touchpad,
        EVRButtonId.k_EButton_SteamVR_Trigger
    };

        EVRButtonId[] axisIds = new EVRButtonId[] {
        EVRButtonId.k_EButton_SteamVR_Touchpad,
        EVRButtonId.k_EButton_SteamVR_Trigger
    };

        void Update()
        {
            foreach (var index in controllerIndices)
            {
                foreach (var buttonId in buttonIds)
                {
                    if (Varjo_SteamVR_Controller.Input(index).GetPressDown(buttonId))
                        Debug.Log(buttonId + " press down");
                    if (Varjo_SteamVR_Controller.Input(index).GetPressUp(buttonId))
                    {
                        Debug.Log(buttonId + " press up");
                        if (buttonId == EVRButtonId.k_EButton_SteamVR_Trigger)
                        {
                            Varjo_SteamVR_Controller.Input(index).TriggerHapticPulse();
                            PrintControllerStatus(index);
                        }
                    }
                    if (Varjo_SteamVR_Controller.Input(index).GetPress(buttonId))
                        Debug.Log(buttonId);
                }

                foreach (var buttonId in axisIds)
                {
                    if (Varjo_SteamVR_Controller.Input(index).GetTouchDown(buttonId))
                        Debug.Log(buttonId + " touch down");
                    if (Varjo_SteamVR_Controller.Input(index).GetTouchUp(buttonId))
                        Debug.Log(buttonId + " touch up");
                    if (Varjo_SteamVR_Controller.Input(index).GetTouch(buttonId))
                    {
                        var axis = Varjo_SteamVR_Controller.Input(index).GetAxis(buttonId);
                        Debug.Log("axis: " + axis);
                    }
                }
            }
        }
    }
}

