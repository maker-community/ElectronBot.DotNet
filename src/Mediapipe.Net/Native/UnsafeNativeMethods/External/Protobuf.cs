// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Runtime.InteropServices;
using Mediapipe.Net.External;

namespace Mediapipe.Net.Native
{
    internal unsafe partial class UnsafeNativeMethods : NativeMethods
    {
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode google_protobuf__SetLogHandler__PF(Protobuf.LogHandler logHandler);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode google_protobuf__ResetLogHandler();

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void mp_api_SerializedProtoArray__delete(void* serializedProtoVectorData, int size);

        #region MessageProto
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_Packet__GetClassificationList(void* packet, out SerializedProto serializedProto);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_Packet__GetClassificationListVector(void* packet, out SerializedProtoVector serializedProtoVector);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_Packet__GetDetection(void* packet, out SerializedProto serializedProto);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_Packet__GetDetectionVector(void* packet, out SerializedProtoVector serializedProtoVector);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_Packet__GetFaceGeometry(void* packet, out SerializedProto serializedProto);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_Packet__GetFaceGeometryVector(void* packet, out SerializedProtoVector serializedProtoVector);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_Packet__GetFrameAnnotation(void* packet, out SerializedProto serializedProto);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_Packet__GetLandmarkList(void* packet, out SerializedProto serializedProto);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_Packet__GetLandmarkListVector(void* packet, out SerializedProtoVector serializedProtoVector);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_Packet__GetNormalizedLandmarkList(void* packet, out SerializedProto serializedProto);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_Packet__GetNormalizedLandmarkListVector(void* packet, out SerializedProtoVector serializedProtoVector);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_Packet__GetNormalizedRect(void* packet, out SerializedProto serializedProto);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_Packet__GetNormalizedRectVector(void* packet, out SerializedProtoVector serializedProtoVector);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_Packet__GetRect(void* packet, out SerializedProto serializedProto);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_Packet__GetRectVector(void* packet, out SerializedProtoVector serializedProtoVector);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_Packet__GetTimedModelMatrixProtoList(void* packet, out SerializedProto serializedProto);
        #endregion
    }
}
