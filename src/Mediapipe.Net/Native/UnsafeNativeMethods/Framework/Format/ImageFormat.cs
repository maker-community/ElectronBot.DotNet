// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Runtime.InteropServices;
using Mediapipe.Net.Framework.Protobuf;

namespace Mediapipe.Net.Native
{
    internal unsafe partial class UnsafeNativeMethods : NativeMethods
    {
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_ImageFrame__(out void* imageFrame);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_ImageFrame__ui_i_i_ui(
            ImageFormat.Types.Format format, int width, int height, uint alignmentBoundary, out void* imageFrame);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_ImageFrame__ui_i_i_i_Pui8(
            ImageFormat.Types.Format format, int width, int height, int widthStep, byte* pixelData,
            out void* imageFrame);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void mp_ImageFrame__delete(void* imageFrame);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_ImageFrame__SetToZero(void* imageFrame);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_ImageFrame__SetAlignmentPaddingAreas(void* imageFrame);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_ImageFrame__CopyToBuffer__Pui8_i(void* imageFrame, byte* buffer, int bufferSize);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_ImageFrame__CopyToBuffer__Pui16_i(void* imageFrame, ushort* buffer, int bufferSize);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_ImageFrame__CopyToBuffer__Pf_i(void* imageFrame, float* buffer, int bufferSize);

        #region StatusOr
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void mp_StatusOrImageFrame__delete(void* statusOrImageFrame);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_StatusOrImageFrame__status(void* statusOrImageFrame, out void* status);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_StatusOrImageFrame__value(void* statusOrImageFrame, out void* imageFrame);
        #endregion

        #region Packet
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp__MakeImageFramePacket__Pif(void* imageFrame, out void* packet);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp__MakeImageFramePacket_At__Pif_Rt(void* imageFrame, void* timestamp, out void* packet);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_Packet__ConsumeImageFrame(void* packet, out void* statusOrImageFrame);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_Packet__GetImageFrame(void* packet, out void* imageFrame);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_Packet__ValidateAsImageFrame(void* packet, out void* status);
        #endregion
    }
}
