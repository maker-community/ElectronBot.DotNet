// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Mediapipe.Net.Native
{
    internal unsafe partial class SafeNativeMethods : NativeMethods
    {
        #region GlContext
        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void* mp_SharedGlContext__get(void* sharedGlContext);

        [SupportedOSPlatform("Linux"), SupportedOSPlatform("Android")]
        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void* mp_GlContext__egl_display(void* glContext);

        [SupportedOSPlatform("Linux"), SupportedOSPlatform("Android")]
        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void* mp_GlContext__egl_config(void* glContext);

        [SupportedOSPlatform("Linux"), SupportedOSPlatform("Android")]
        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void* mp_GlContext__egl_context(void* glContext);

        [SupportedOSPlatform("IOS")]
        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void* mp_GlContext__eagl_context(void* glContext);

        [SupportedOSPlatform("OSX")]
        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void* mp_GlContext__nsgl_context(void* glContext);

        [SupportedOSPlatform("OSX")]
        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void* mp_GlContext__nsgl_pixel_format(void* glContext);

        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern byte mp_GlContext__IsCurrent(void* glContext);

        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern int mp_GlContext__gl_major_version(void* glContext);

        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern int mp_GlContext__gl_minor_version(void* glContext);

        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern long mp_GlContext__gl_finish_count(void* glContext);
        #endregion

        #region GlSyncToken
        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void* mp_GlSyncToken__get(void* glSyncToken);
        #endregion
    }
}
