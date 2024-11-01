// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Runtime.InteropServices;
using Mediapipe.Net.Gpu;

namespace Mediapipe.Net.Native
{
    internal unsafe partial class UnsafeNativeMethods : NativeMethods
    {
        #region GlTextureBuffer
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_GlTextureBuffer__WaitUntilComplete(void* glTextureBuffer);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_GlTextureBuffer__WaitOnGpu(void* glTextureBuffer);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_GlTextureBuffer__Reuse(void* glTextureBuffer);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_GlTextureBuffer__Updated__Pgst(void* glTextureBuffer, void* prodToken);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_GlTextureBuffer__DidRead__Pgst(void* glTextureBuffer, void* consToken);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_GlTextureBuffer__WaitForConsumers(void* glTextureBuffer);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_GlTextureBuffer__WaitForConsumersOnGpu(void* glTextureBuffer);
        #endregion

        #region SharedGlTextureBuffer
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void mp_SharedGlTextureBuffer__delete(void* glTextureBuffer);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void mp_SharedGlTextureBuffer__reset(void* glTextureBuffer);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_SharedGlTextureBuffer__ui_ui_i_i_ui_PF_PSgc(
            uint target, uint name, int width, int height, GpuBufferFormat format,
            GlTextureBuffer.DeletionCallback deletionCallback,
            void* producerContext, out void* sharedGlTextureBuffer);
        #endregion
    }
}
