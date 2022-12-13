// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Mediapipe.Net.Gpu;

namespace Mediapipe.Net.Native
{
    internal unsafe partial class UnsafeNativeMethods : NativeMethods
    {
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_GlCalculatorHelper__(out void* glCalculatorHelper);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void mp_GlCalculatorHelper__delete(void* glCalculatorHelper);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_GlCalculatorHelper__InitializeForTest__Pgr(void* glCalculatorHelper, void* gpuResources);

        // TODO: Make it ba a member of GlCalculatorHelper
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_GlCalculatorHelper__RunInGlContext__PF(
            void* glCalculatorHelper, GlCalculatorHelper.NativeGlStatusFunction glFunc, out void* status);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_GlCalculatorHelper__CreateSourceTexture__Rif(
            void* glCalculatorHelper, void* imageFrame, out void* glTexture);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_GlCalculatorHelper__CreateSourceTexture__Rgb(
            void* glCalculatorHelper, void* gpuBuffer, out void* glTexture);

        [SupportedOSPlatform("IOS")]
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_GlCalculatorHelper__CreateSourceTexture__Rgb_i(
            void* glCalculatorHelper, void* gpuBuffer, int plane, out void* glTexture);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_GlCalculatorHelper__CreateDestinationTexture__i_i_ui(
            void* glCalculatorHelper, int outputWidth, int outputHeight, GpuBufferFormat formatCode, out void* glTexture);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_GlCalculatorHelper__CreateDestinationTexture__Rgb(
            void* glCalculatorHelper, void* gpuBuffer, out void* glTexture);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_GlCalculatorHelper__BindFrameBuffer__Rtexture(void* glCalculatorHelper, void* glTexture);
    }
}
