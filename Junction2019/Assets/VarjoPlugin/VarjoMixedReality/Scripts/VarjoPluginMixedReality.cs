﻿// Copyright 2019 Varjo Technologies Oy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Varjo
{
    public class VarjoPluginMR
    {
        /// <summary>
        /// Distorted color stream from VST cameras.
        /// </summary>
        public static readonly VarjoDistortedColorStream distortedColorStream = new VarjoDistortedColorStream();

        /// <summary>
        /// Environmental lighting cubemap stream.
        /// </summary>
        public static readonly VarjoEnvironmentCubemapStream environmentCubemapStream = new VarjoEnvironmentCubemapStream();

        [DllImport("VarjoUnity")]
        private static extern bool MRIsAvailable();

        [DllImport("VarjoUnity")]
        private static extern IntPtr GetSession();

        [DllImport("VarjoUnity")]
        private static extern bool MRSupportsDataStream(VarjoStreamType streamType);

        [DllImport("VarjoUnity")]
        private static extern bool MRStartDataStream(VarjoStreamType streamType, VarjoStreamCallback callback, IntPtr userdata);

        [DllImport("VarjoUnity")]
        private static extern void MRStopDataStream(VarjoStreamType streamType);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        private static extern void varjo_MRSetVideoRender(IntPtr session, bool enabled);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        private static extern void varjo_MRSetVideoDepthEstimation(IntPtr session, bool enabled);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        private static extern void varjo_MRSetVRViewOffset(IntPtr session, double percentage);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        private static extern void varjo_LockDataStreamBuffer(IntPtr session, long id);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        private static extern void varjo_UnlockDataStreamBuffer(IntPtr session, long id);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        private static extern VarjoCameraIntrinsics varjo_GetCameraIntrinsics(IntPtr session, long id, long frameNumber, long channelIndex);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        private static extern VarjoPlugin.Matrix varjo_GetCameraExtrinsics(IntPtr session, long id, long frameNumber, long index);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        private static extern long varjo_GetBufferId(IntPtr session, long id, long frameNumber, long index);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        private static extern VarjoBufferMetadata varjo_GetBufferMetadata(IntPtr session, long id);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        private static extern VarjoTexture varjo_GetBufferGPUData(IntPtr session, long id);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        private static extern IntPtr varjo_GetBufferCPUData(IntPtr session, long id);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        private static extern int varjo_MRLockCameraConfig(IntPtr session);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        private static extern void varjo_MRUnlockCameraConfig(IntPtr session);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        private static extern int varjo_MRGetCameraPropertyModeCount(IntPtr session, VarjoCameraPropertyType prop);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        private static extern void varjo_MRGetCameraPropertyModes(IntPtr session, VarjoCameraPropertyType prop, [In, Out] VarjoCameraPropertyMode[] modes, int maxSize);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        private static extern long varjo_MRGetCameraPropertyMode(IntPtr session, VarjoCameraPropertyType type);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        private static extern void varjo_MRSetCameraPropertyMode(IntPtr session, VarjoCameraPropertyType type, VarjoCameraPropertyMode mode);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        private static extern long varjo_MRGetCameraPropertyConfigType(IntPtr session, VarjoCameraPropertyType prop);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        private static extern int varjo_MRGetCameraPropertyValueCount(IntPtr session, VarjoCameraPropertyType prop);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        private static extern int varjo_MRGetCameraPropertyValues(IntPtr session, VarjoCameraPropertyType prop, [In, Out] VarjoCameraPropertyValue[] values, int maxSize);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        private static extern VarjoCameraPropertyValue varjo_MRGetCameraPropertyValue(IntPtr session, VarjoCameraPropertyType type);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        private static extern void varjo_MRSetCameraPropertyValue(IntPtr session, VarjoCameraPropertyType type, ref VarjoCameraPropertyValue value);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        private static extern void varjo_MRResetCameraProperty(IntPtr session, VarjoCameraPropertyType type);

        [DllImport("VarjoLib", CharSet = CharSet.Auto)]
        private static extern void varjo_MRResetCameraProperties(IntPtr session);

        private static bool IsMRReady()
        {
            if (!VarjoPlugin.SessionValid)
            {
                return false;
            }

            if (!MRIsAvailable())
            {
                Debug.LogError("Mixed reality hardware not available.");
                return false;
            }

            return true;
        }

        private static bool CheckError()
        {
            int error = VarjoPlugin.GetError();
            if (error != 0)
            {
                Debug.LogWarning(VarjoPlugin.GetErrorMsg(error));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Starts video see-through rendering.
        /// </summary>
        /// <returns>True, if VST rendering was started successfully.</returns>
        public static bool StartRender()
        {
            if (!IsMRReady()) return false;
            varjo_MRSetVideoRender(GetSession(), true);
            return CheckError();
        }

        /// <summary>
        /// Stops video see-through rendering.
        /// </summary>
        public static void StopRender()
        {
            if (!IsMRReady()) return;
            varjo_MRSetVideoRender(GetSession(), false);
        }

        /// <summary>
        /// Enables depth estimation from VST images.
        /// </summary>
        /// <returns>True, if depth estimation was enabled successfully.</returns>
        public static bool EnableDepthEstimation()
        {
            if (!IsMRReady()) return false;
            varjo_MRSetVideoDepthEstimation(GetSession(), true);
            return CheckError();
        }

        /// <summary>
        /// Disables depth estimation from VST images.
        /// </summary>
        public static void DisableDepthEstimation()
        {
            if (!IsMRReady()) return;
            varjo_MRSetVideoDepthEstimation(GetSession(), false);
        }

        /// <summary>
        /// Change virtual camera rendering position between users eyes and video see through cameras.
        /// </summary>
        /// <remarks>
        /// The scene is rendered twice from the position of the users eyes. In full VR the eye position corresponds to the physical
        /// position of the users eyes. When using video see through there is a physical offset between the users eyes and the
        /// stereo camera pair. So in contrast to full VR, when rendering in MR mode: To remove stereo disparity problems between the
        /// real and virtual world and prevent 'floating' of the VR objects anchored to the real world, the scene should be rendered
        /// from the physical position of the camera pair in most cases. This is the default mode and corresponds to setting eye offset
        /// 'percentage' to 1.0.
        ///
        /// But there can be scenes that are predominantly VR where it makes sense to render the scene using the VR eye position.
        /// e.g. The scene only contains a small 'magic window' to view the real world or the real world is only shown as a backdrop.
        ///
        /// This function can be used to switch the rendering position. It can be used for smooth interpolated change in case it
        /// needs to be done the middle of the scene.
        ///
        /// Note: This setting is ignored if eye reprojection is enabled (#varjo_CameraPropertyType_EyeReprojection). In this case
        /// the rendering is always done from the users eye position (full VR position, corresponds to 'percentage' 0.0).
        /// </remarks>
        /// <param name="session">
        /// Varjo session handle.
        /// </param>
        /// <param name="percentage">
        ///  [0.0, 1.0] Linear interpolation of the rendering position between the position of HMD users eyes and the video see through camera position.
        /// </param>
        public static void SetVRViewOffset(double percentage)
        {
            if (!IsMRReady()) return;
            varjo_MRSetVRViewOffset(GetSession(), percentage);
        }

        /// <summary>
        /// Locks camera configuration. This is required for the client application to be able to modify
        /// camera parameters.
        /// </summary>
        /// <returns>True, if lock acquired successfully. Otherwise false.</returns>
        public static bool LockCameraConfig()
        {
            if (!IsMRReady()) return false;
            return (varjo_MRLockCameraConfig(GetSession()) == 1);
        }

        /// <summary>
        /// Unlocks camera configuration. Client should use this when it no longer want to change camera
        /// parameters.
        /// </summary>
        public static void UnlockCameraConfig()
        {
            if (!IsMRReady()) return;
            varjo_MRUnlockCameraConfig(GetSession());
        }

        /// <summary>
        /// Retrieves the number of available modes for the given camera property.
        /// </summary>
        /// <param name="type">The camera property type.</param>
        /// <returns>The number of modes for the camera property.</returns>
        public static int GetCameraPropertyModeCount(VarjoCameraPropertyType type)
        {
            if (!IsMRReady()) return 0;
            return varjo_MRGetCameraPropertyModeCount(GetSession(), type);
        }

        /// <summary>
        /// Retrieves all available modes for the given camera property.
        /// </summary>
        /// <param name="type">The camera property type.</param>
        /// <param name="modes">The available modes for the camera property.</param>
        /// <returns>True, if no errors. Otherwise false.</returns>
        public static bool GetCameraPropertyModes(VarjoCameraPropertyType type, out List<VarjoCameraPropertyMode> modes)
        {
            if (!IsMRReady())
            {
                modes = new List<VarjoCameraPropertyMode>();
                return false;
            }

            int count = GetCameraPropertyModeCount(type);
            VarjoCameraPropertyMode[] modesArray = new VarjoCameraPropertyMode[count];
            varjo_MRGetCameraPropertyModes(GetSession(), type, modesArray, count);
            modes = modesArray.ToList();
            return CheckError();
        }

        /// <summary>
        /// Retrieves the current mode for the camera property.
        /// </summary>
        /// <param name="type">The camera property type.</param>
        /// <param name="mode">The current mode for the camera property.</param>
        /// <returns>True, if no errors. Otherwise false.</returns>
        public static bool GetCameraPropertyMode(VarjoCameraPropertyType type, out VarjoCameraPropertyMode mode)
        {
            if (!IsMRReady())
            {
                mode = VarjoCameraPropertyMode.Off;
                return false;
            }

            mode = (VarjoCameraPropertyMode)varjo_MRGetCameraPropertyMode(GetSession(), type);
            return CheckError();
        }

        /// <summary>
        /// Sets the current mode for the camera property.
        /// </summary>
        /// <param name="type">The camera property type.</param>
        /// <param name="mode">The mode to set.</param>
        /// <returns>True, if no errors. Otherwise false.</returns>
        public static bool SetCameraPropertyMode(VarjoCameraPropertyType type, VarjoCameraPropertyMode mode)
        {
            if (!IsMRReady()) return false;
            varjo_MRSetCameraPropertyMode(GetSession(), type, mode);
            return CheckError();
        }

        /// <summary>
        /// Retrieves the camera property's configuration type, i.e. does it accept only selected list values or
        /// a range of values.
        /// </summary>
        /// <param name="prop">The camera property type.</param>
        /// <param name="configType">The camera property's configuration type.</param>
        /// <returns>True, if no errors. Otherwise false.</returns>
        public static bool GetCameraPropertyConfigType(VarjoCameraPropertyType prop, out VarjoCameraPropertyConfigType configType)
        {
            if (!IsMRReady())
            {
                configType = VarjoCameraPropertyConfigType.List;
                return false;
            }

            configType = (VarjoCameraPropertyConfigType)varjo_MRGetCameraPropertyConfigType(GetSession(), prop);
            return CheckError();
        }

        /// <summary>
        /// Retrieves the number of possible values for the camera property.
        /// </summary>
        /// <param name="type">The camera property type.</param>
        /// <returns>The number of possible values for the camera property.</returns>
        public static int GetCameraPropertyValueCount(VarjoCameraPropertyType type)
        {
            if (!IsMRReady()) return 0;
            return varjo_MRGetCameraPropertyValueCount(GetSession(), type);
        }

        /// <summary>
        /// Retrieves all possible values for the given camera property.
        /// </summary>
        /// <param name="type">The camera property type.</param>
        /// <param name="values">The list of possible values for the camera property.</param>
        /// <returns>True, if no errors. Otherwise false.</returns>
        public static bool GetCameraPropertyValues(VarjoCameraPropertyType type, out List<VarjoCameraPropertyValue> values)
        {
            if (!IsMRReady())
            {
                values = new List<VarjoCameraPropertyValue>();
                return false;
            }

            int count = GetCameraPropertyValueCount(type);
            VarjoCameraPropertyValue[] valueArray = new VarjoCameraPropertyValue[count];
            varjo_MRGetCameraPropertyValues(GetSession(), type, valueArray, count);
            values = valueArray.ToList();
            return CheckError();
        }

        /// <summary>
        /// Retrieves the current value for the given camera property.
        /// </summary>
        /// <param name="type">The camera property type.</param>
        /// <param name="value">The current value of the camera property.</param>
        /// <returns>True, if no errors. Otherwise false.</returns>
        public static bool GetCameraPropertyValue(VarjoCameraPropertyType type, out VarjoCameraPropertyValue value)
        {
            if (!IsMRReady())
            {
                value = new VarjoCameraPropertyValue();
                return false;
            }

            value = varjo_MRGetCameraPropertyValue(GetSession(), type);
            return CheckError();
        }

        /// <summary>
        /// Sets the current value for the given camera property.
        /// </summary>
        /// <param name="type">The camera property type.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>True, if no errors. Otherwise false.</returns>
        public static bool SetCameraPropertyValue(VarjoCameraPropertyType type, VarjoCameraPropertyValue value)
        {
            if (!IsMRReady()) return false;
            varjo_MRSetCameraPropertyValue(GetSession(), type, ref value);
            return CheckError();
        }

        /// <summary>
        /// Resets the given camera property back to its default.
        /// </summary>
        /// <param name="type">The camera property type.</param>
        public static void ResetCameraProperty(VarjoCameraPropertyType type)
        {
            if (!IsMRReady()) return;
            varjo_MRResetCameraProperty(GetSession(), type);
        }

        /// <summary>
        /// Resets all camera properties back to their defaults.
        /// </summary>
        public static void ResetCameraProperties()
        {
            if (!IsMRReady()) return;
            varjo_MRResetCameraProperties(GetSession());
        }

        internal static bool GetDataStreamBufferId(long streamId, long frameNumber, long channelIndex, out long bufferId)
        {
            if (!IsMRReady())
            {
                bufferId = -1;
                return false;
            }
            bufferId = varjo_GetBufferId(GetSession(), streamId, frameNumber, channelIndex);
            return CheckError();
        }

        internal static bool LockDataStreamBuffer(long id)
        {
            if (!IsMRReady())
            {
                return false;
            }
            varjo_LockDataStreamBuffer(GetSession(), id);
            return CheckError();
        }

        internal static bool GetBufferMetadata(long id, out VarjoBufferMetadata metadata)
        {
            if (!IsMRReady())
            {
                metadata = new VarjoBufferMetadata();
                return false;
            }
            metadata = varjo_GetBufferMetadata(GetSession(), id);
            return CheckError();
        }

        internal static bool GetBufferCPUData(long id, out IntPtr cpuData)
        {
            if (!IsMRReady())
            {
                cpuData = IntPtr.Zero;
                return false;
            }
            cpuData = varjo_GetBufferCPUData(GetSession(), id);
            return CheckError();
        }

        internal static bool GetBufferGPUData(long id, out VarjoTexture gpuData)
        {
            if (!IsMRReady())
            {
                gpuData = new VarjoTexture();
                return false;
            }
            gpuData = varjo_GetBufferGPUData(GetSession(), id);
            return CheckError();
        }

        internal static void UnlockDataStreamBuffer(long id)
        {
            if (!IsMRReady()) return;
            varjo_UnlockDataStreamBuffer(GetSession(), id);
        }

        internal static bool SupportsDataStream(VarjoStreamType type)
        {
            if (!IsMRReady()) return false;
            return MRSupportsDataStream(type);
        }

        internal static bool StartDataStream(VarjoStreamType type, VarjoStreamCallback callback)
        {
            if (!IsMRReady()) return false;
            return MRStartDataStream(type, callback, IntPtr.Zero);
        }

        internal static void StopDataStream(VarjoStreamType type)
        {
            if (!IsMRReady()) return;
            MRStopDataStream(type);
        }
    }
}
