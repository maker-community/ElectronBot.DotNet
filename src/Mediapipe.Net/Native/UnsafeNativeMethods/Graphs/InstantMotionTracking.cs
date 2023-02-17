// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Runtime.InteropServices;
using Mediapipe.Net.Graphs.InstantMotionTracking;

namespace Mediapipe.Net.Native
{
    internal unsafe partial class UnsafeNativeMethods : NativeMethods
    {
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp__MakeAnchor3dVectorPacket__PA_i(Anchor3d[] value, int size, out void* packet);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp__MakeAnchor3dVectorPacket_At__PA_i_Rt(Anchor3d[] value, int size, void* timestamp, out void* packet);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void mp_Anchor3dArray__delete(void* anchorVectorData);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_Packet__GetAnchor3dVector(void* packet, out Anchor3dVector anchorVector);
    }
}
