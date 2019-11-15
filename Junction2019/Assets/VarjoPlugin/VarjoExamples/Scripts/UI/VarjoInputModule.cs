// Copyright 2019 Varjo Technologies Oy. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Varjo;
using Varjo.Valve;
using Varjo.Valve.VR;

/// <summary>
/// Replacement for Standalone Input Module.
/// For use with world space canvases in vr.
/// Handles keyboard, headset and hand controller for canvas.
/// Recommended to use with Varjo pointer.
/// Requires dummy camera for pointing. Rendering of the camera can be disabled.
/// If Varjo pointer is used. Add dummy camera to it.
/// </summary>
namespace UnityEngine.EventSystems
{
    public class VarjoInputModule : PointerInputModule
    {
        public enum InputMethod
        {
            KEYBOARD,
            HEADSET,
            CONTROLLER
        }

        [Header("Button used for pressing ui elements")]
        public string button = "Jump";

        [Header("Should we use Varjo headset application button for selector")]
        public bool useApplicationButton = true;

        [Header("Should we use controller trigger for selector")]
        public bool useControllerTrigger = true;

        [Header("Camera to point at canvas (can be disabled)")]
        public Camera targetCamera;

        [Header("Targeted world space canvas (searched automatically if missing)")]
        public Canvas targetCanvas;

        public Vector3 PointerWorldPosition { get; private set; }
        public Vector3 PointerWorldPositionNormal { get; private set; }
        public bool PointerHitCanvas { get; private set; }
        public InputMethod LastInputMethod { get; private set; }

        // We use mouse state to store the state from our supported input devices
        MouseState mouseState = new MouseState();
        PointerEventData pointerEventData;
        bool buttonHeld;

        protected override void Start()
        {
            base.Start();

            if (targetCamera == null)
            {
                Debug.LogError("No camera for VarjoInputModule. Disabling.", gameObject);
                enabled = false;
                return;
            }

            // Search for canvas if we don't have one
            if (targetCanvas == null)
            {
                targetCanvas = GameObject.FindObjectOfType<Canvas>();
            }

            if (targetCanvas == null)
            {
                Debug.LogError("No canvas for VarjoInputModule. Disabling.", gameObject);
                enabled = false;
                return;
            }

            if (targetCanvas.renderMode != RenderMode.WorldSpace)
            {
                Debug.LogError("VarjoInputModule canvas isn't in world space. Disabling.", gameObject);
                enabled = false;
                return;
            }

            // Make sure canvas is using our dummy camera
            targetCanvas.worldCamera = targetCamera;

            // Use controller as default input
            LastInputMethod = InputMethod.CONTROLLER;

            // Listen when controller comes available and default to it when it happends
            Varjo_SteamVR_Events.DeviceConnected.Listen(OnControllerConnected);
        }

        public override void Process()
        {
            // Try to get pointer for this input module
            pointerEventData = new PointerEventData(EventSystem.current);
            GetPointerData(kMouseLeftId, out pointerEventData, true);

            pointerEventData.Reset();
            pointerEventData.delta = Vector2.zero;
            pointerEventData.position = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            pointerEventData.scrollDelta = Vector2.zero;
            pointerEventData.button = PointerEventData.InputButton.Left;

            // Raycast pointer to canvas
            m_RaycastResultCache.Clear();
            eventSystem.RaycastAll(pointerEventData, m_RaycastResultCache);
            pointerEventData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
            PointerHitCanvas = m_RaycastResultCache.Count > 0;
            UpdatePointerWorldPosition();

            // Process presses & events
            mouseState.SetButtonState(PointerEventData.InputButton.Left, GetPressState(), pointerEventData);
            MouseButtonEventData buttonState = mouseState.GetButtonState(PointerEventData.InputButton.Left).eventData;

            if (buttonState.PressedThisFrame())
            {
                ButtonPress(buttonState.buttonData);
            }

            if (buttonState.ReleasedThisFrame())
            {
                ButtonRelease(buttonState.buttonData);
            }

            ProcessMove(buttonState.buttonData);

            if (buttonHeld)
            {
                ProcessDrag(buttonState.buttonData);
            }
        }

        PointerEventData.FramePressState GetPressState()
        {
            bool buttonPress = false;
            bool buttonRelease = false;

            // Get keyboard press
            if (button != "")
            {
                buttonPress = false;
                if (Input.GetButtonDown(button))
                {
                    buttonPress = true;
                    LastInputMethod = InputMethod.KEYBOARD;
                }
                buttonRelease = Input.GetButtonUp(button);
            }

            // Get headset presse
            if (useApplicationButton)
            {
                if (VarjoManager.Instance.GetButtonDown())
                {
                    buttonPress = true;
                    LastInputMethod = InputMethod.HEADSET;
                }
                buttonRelease = VarjoManager.Instance.GetButtonUp() || buttonRelease;
            }

            // Get hand controller trigger
            if (useControllerTrigger && OpenVR.System != null)
            {
                int controllerIndex = (int)OpenVR.System.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand);
                if (controllerIndex >= 0)
                {
                    Varjo_SteamVR_Controller.Device controller = Varjo_SteamVR_Controller.Input(controllerIndex);

                    if (controller.connected)
                    {
                        if (controller.GetHairTriggerDown())
                        {
                            buttonPress = true;
                            LastInputMethod = InputMethod.CONTROLLER;
                        }
                        buttonRelease = controller.GetHairTriggerUp() || buttonRelease;
                    }
                }
            }

            // Return current state
            if (buttonPress && buttonRelease)
            {
                return PointerEventData.FramePressState.PressedAndReleased;
            }
            else if (buttonPress)
            {
                buttonHeld = true;
                return PointerEventData.FramePressState.Pressed;
            }
            else if (buttonRelease)
            {
                buttonHeld = false;
                return PointerEventData.FramePressState.Released;
            }
            return PointerEventData.FramePressState.NotChanged;
        }

        void ButtonPress(PointerEventData pointerEvent)
        {
            GameObject target = pointerEvent.pointerCurrentRaycast.gameObject;

            pointerEvent.eligibleForClick = true;
            pointerEvent.delta = Vector2.zero;
            pointerEvent.dragging = false;
            pointerEvent.useDragThreshold = true;
            pointerEvent.pressPosition = pointerEvent.position;
            pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;
            pointerEvent.clickCount = 1;

            DeselectIfSelectionChanged(target, pointerEvent);

            if (pointerEvent.pointerEnter != target)
            {
                HandlePointerExitAndEnter(pointerEvent, target);
                pointerEvent.pointerEnter = target;
            }

            GameObject pressHandler = ExecuteEvents.ExecuteHierarchy(target, pointerEvent, ExecuteEvents.pointerDownHandler);

            if (pressHandler == null)
            {
                pressHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(target);
            }

            pointerEvent.pointerPress = pressHandler;
            pointerEvent.rawPointerPress = target;
            pointerEvent.clickTime = Time.unscaledTime;

            pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(target);

            if (pointerEvent.pointerDrag != null)
            {
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
            }
        }

        void ButtonRelease(PointerEventData pointerEvent)
        {
            GameObject target = pointerEvent.pointerCurrentRaycast.gameObject;

            ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
            GameObject releaseHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(target);

            if (pointerEvent.pointerPress == releaseHandler && pointerEvent.eligibleForClick)
            {
                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
            }
            else if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
            {
                ExecuteEvents.ExecuteHierarchy(target, pointerEvent, ExecuteEvents.dropHandler);
            }

            pointerEvent.eligibleForClick = false;
            pointerEvent.pointerPress = null;
            pointerEvent.rawPointerPress = null;

            if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
            {
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
            }

            pointerEvent.dragging = false;
            pointerEvent.pointerDrag = null;

            ExecuteEvents.ExecuteHierarchy(pointerEvent.pointerEnter, pointerEvent, ExecuteEvents.pointerExitHandler);
            pointerEvent.pointerEnter = null;

            pointerEvent.clickCount = 0;
        }

        void OnControllerConnected(int index, bool connected)
        {
            ETrackedDeviceClass deviceClass = OpenVR.System.GetTrackedDeviceClass((uint)index);
            if (deviceClass == ETrackedDeviceClass.Controller)
            {
                LastInputMethod = connected ? InputMethod.CONTROLLER : InputMethod.HEADSET;
            }
        }

        void UpdatePointerWorldPosition()
        {
            if (PointerHitCanvas)
            {
                Plane plane = new Plane(-targetCanvas.transform.forward, targetCanvas.transform.position);
                Ray ray = new Ray(targetCamera.ScreenToWorldPoint(pointerEventData.position), targetCamera.transform.forward);
                float intersectDistance = 0.0f;
                plane.Raycast(ray, out intersectDistance);
                PointerWorldPosition = targetCamera.ScreenToWorldPoint(pointerEventData.position) + (targetCamera.transform.forward * intersectDistance);
                PointerWorldPositionNormal = -targetCanvas.transform.forward;
            }
            else
            {
                PointerWorldPosition = Vector3.zero;
                PointerWorldPositionNormal = -targetCamera.transform.forward;
            }
        }
    }
}
