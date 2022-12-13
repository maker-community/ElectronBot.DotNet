// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Mediapipe.Net.Native
{
    internal unsafe partial class UnsafeNativeMethods : NativeMethods
    {
        [SupportedOSPlatform("Linux"), SupportedOSPlatform("Android")]
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_GpuBuffer__PSgtb(void* glTextureBuffer, out void* gpuBuffer);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void mp_GpuBuffer__delete(void* gpuBuffer);

        #region StatusOr
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void mp_StatusOrGpuBuffer__delete(void* statusOrGpuBuffer);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_StatusOrGpuBuffer__status(void* statusOrGpuBuffer, out void* status);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_StatusOrGpuBuffer__value(void* statusOrGpuBuffer, out void* gpuBuffer);
        #endregion

        #region Packet
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp__MakeGpuBufferPacket__Rgb(void* gpuBuffer, out void* packet);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp__MakeGpuBufferPacket_At__Rgb_Rts(void* gpuBuffer, void* timestamp, out void* packet);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_Packet__ConsumeGpuBuffer(void* packet, out void* statusOrGpuBuffer);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_Packet__GetGpuBuffer(void* packet, out void* gpuBuffer);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_Packet__ValidateAsGpuBuffer(void* packet, out void* status);
        #endregion
    }
}
