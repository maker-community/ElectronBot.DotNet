// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Mediapipe.Net.Gpu;

namespace Mediapipe.Net.Native
{
    internal unsafe partial class SafeNativeMethods : NativeMethods
    {
        #region GlTextureBuffer
        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern uint mp_GlTextureBuffer__name(void* glTextureBuffer);

        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern uint mp_GlTextureBuffer__target(void* glTextureBuffer);

        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern int mp_GlTextureBuffer__width(void* glTextureBuffer);

        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern int mp_GlTextureBuffer__height(void* glTextureBuffer);

        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern GpuBufferFormat mp_GlTextureBuffer__format(void* glTextureBuffer);

        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void* mp_GlTextureBuffer__GetProducerContext(void* glTextureBuffer);
        #endregion

        #region SharedGlTextureBuffer
        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void* mp_SharedGlTextureBuffer__get(void* glTextureBuffer);
        #endregion
    }
}
