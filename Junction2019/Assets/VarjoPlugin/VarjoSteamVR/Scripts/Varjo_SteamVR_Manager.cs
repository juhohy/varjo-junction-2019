//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Handles rendering of all SteamVR_Cameras
//
//=============================================================================

// Modified By Varjo to SteamVR manager
// Varjo doesn't use OpenVR rendering

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Varjo.Valve.VR;

namespace Varjo
{
    public class Varjo_SteamVR_Manager : MonoBehaviour
    {
        public ETrackingUniverseOrigin TrackingSpace { get; private set; }

        static public EVREye eye { get; private set; }

        static private Varjo_SteamVR_Manager _instance;
        static public Varjo_SteamVR_Manager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<Varjo_SteamVR_Manager>();

                    if (_instance == null)
                        _instance = new GameObject("[SteamVR]").AddComponent<Varjo_SteamVR_Manager>();
                }
                return _instance;
            }
        }

        void OnDestroy()
        {
            _instance = null;
        }

        //static private bool isQuitting;
        void OnApplicationQuit()
        {
            //isQuitting = true;
            Varjo_SteamVR.SafeDispose();
        }

        public TrackedDevicePose_t[] poses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
        public TrackedDevicePose_t[] gamePoses = new TrackedDevicePose_t[0];

        void OnQuit(VREvent_t vrEvent)
        {
#if UNITY_EDITOR
            foreach (System.Reflection.Assembly a in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                var t = a.GetType("UnityEditor.EditorApplication");
                if (t != null)
                {
                    t.GetProperty("isPlaying").SetValue(null, false, null);
                    break;
                }
            }
#else
			Application.Quit();
#endif
        }

        void OnEnable()
        {
			StopAllCoroutines();
			StartCoroutine(Initialize());

        }

        void OnDisable()
        {
			StopAllCoroutines();

			if (OpenVR.System == null)
				return;

            Varjo_SteamVR_Events.System(EVREventType.VREvent_Quit).Remove(OnQuit);
#if UNITY_2017_1_OR_NEWER
            Application.onBeforeRender -= OnBeforeRender;
#else
			Camera.onPreCull -= OnCameraPreCull;
#endif
        }

		IEnumerator Initialize() {
            var error = EVRInitError.None;

			// No need to initialize OpenVR until we have a valid session
			while (!VarjoPlugin.SessionValid) {
				yield return new WaitForSeconds(1.0f);
			}

			OpenVR.Init(ref error, EVRApplicationType.VRApplication_Background);

			if (error != EVRInitError.None) {
                if (error == EVRInitError.Init_NoServerForBackgroundApp)
                {
                    Debug.LogError("Failed to initialize OpenVR. OpenVR controllers will not work. Restart Unity Editor and try again.");
                    yield break;
                }
                else
                {
                    Debug.LogWarning("Failed to initialize OpenVR. Entering into poll mode...");
                    do
                    {
                        OpenVR.Init(ref error, EVRApplicationType.VRApplication_Background);
                        yield return new WaitForSeconds(3.0f);
                    } while (error != EVRInitError.None);
                }
			}

			Debug.Log("Varjo_SteamVR_Manager initialized succesfully");

			Varjo_SteamVR_Events.System(EVREventType.VREvent_Quit).Listen(OnQuit);
#if UNITY_2017_1_OR_NEWER
            Application.onBeforeRender += OnBeforeRender;
#else
			Camera.onPreCull += OnCameraPreCull;
#endif
            // We should always use standing tracking with Varjo
            TrackingSpace = ETrackingUniverseOrigin.TrackingUniverseStanding;

            var vr = Varjo_SteamVR.instance;
            if (vr == null)
            {
                enabled = false;
                yield break;
            }
		}

        public void UpdatePoses()
        {
			if (!VarjoPlugin.SessionValid || OpenVR.System == null)
				return;

            OpenVR.System.GetDeviceToAbsoluteTrackingPose(TrackingSpace, 0.0f, poses);

            Varjo_SteamVR_Events.NewPoses.Send(poses);
            Varjo_SteamVR_Events.NewPosesApplied.Send();

            // We are not using OpenVR compositor so we can't fetch poses from there
            /*var compositor = OpenVR.Compositor;
            if (compositor != null)
            {
                compositor.GetLastPoses(poses, gamePoses);
                SteamVR_Events.NewPoses.Send(poses);
                SteamVR_Events.NewPosesApplied.Send();
            }*/
        }

#if UNITY_2017_1_OR_NEWER
        void OnBeforeRender() { UpdatePoses(); }
#else
		void OnCameraPreCull(Camera cam)
		{
			// Only update poses on the first camera per frame.
			if (Time.frameCount != lastFrameCount)
			{
				lastFrameCount = Time.frameCount;
				UpdatePoses();
			}
		}
		static int lastFrameCount = -1;
#endif

        void Update()
        {
			if (!VarjoPlugin.SessionValid || OpenVR.System == null)
				return;

            // Force controller update in case no one else called this frame to ensure prevState gets updated.
            Varjo_SteamVR_Controller.Update();

            // Dispatch any OpenVR events.
            var system = OpenVR.System;
            if (system != null)
            {
                var vrEvent = new VREvent_t();
                var size = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(VREvent_t));
                for (int i = 0; i < 64; i++)
                {
                    if (!system.PollNextEvent(ref vrEvent, size))
                        break;

                    switch ((EVREventType)vrEvent.eventType)
                    {
                        case EVREventType.VREvent_ShowRenderModels:
                            Varjo_SteamVR_Events.HideRenderModels.Send(false);
                            break;
                        case EVREventType.VREvent_HideRenderModels:
                            Varjo_SteamVR_Events.HideRenderModels.Send(true);
                            break;
                        default:
                            Varjo_SteamVR_Events.System((EVREventType)vrEvent.eventType).Send(vrEvent);
                            break;
                    }
                }
            }

        }
    }
}

