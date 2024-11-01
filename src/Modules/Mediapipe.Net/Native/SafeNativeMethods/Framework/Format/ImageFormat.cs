// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Mediapipe.Net.Framework.Protobuf;

namespace Mediapipe.Net.Native
{
    internal unsafe partial class SafeNativeMethods : NativeMethods
    {
        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern byte mp_ImageFrame__IsEmpty(void* imageFrame);

        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern byte mp_ImageFrame__IsContiguous(void* imageFrame);

        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_ImageFrame__IsAligned__ui(
            void* imageFrame, uint alignmentBoundary, out bool value);

        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern ImageFormat.Types.Format mp_ImageFrame__Format(void* imageFrame);

        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern int mp_ImageFrame__Width(void* imageFrame);

        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern int mp_ImageFrame__Height(void* imageFrame);

        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_ImageFrame__ChannelSize(void* imageFrame, out int value);

        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_ImageFrame__NumberOfChannels(void* imageFrame, out int value);

        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_ImageFrame__ByteDepth(void* imageFrame, out int value);

        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern int mp_ImageFrame__WidthStep(void* imageFrame);

        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern byte* mp_ImageFrame__MutablePixelData(void* imageFrame);

        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern int mp_ImageFrame__PixelDataSize(void* imageFrame);

        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_ImageFrame__PixelDataSizeStoredContiguously(void* imageFrame, out int value);

        #region StatusOr
        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern byte mp_StatusOrImageFrame__ok(void* statusOrImageFrame);
        #endregion
    }
}
