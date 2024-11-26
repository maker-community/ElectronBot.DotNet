// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Mediapipe.Net.Native
{
    internal unsafe partial class UnsafeNativeMethods : NativeMethods
    {
        #region GlContext
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void mp_SharedGlContext__delete(void* sharedGlContext);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void mp_SharedGlContext__reset(void* sharedGlContext);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_GlContext_GetCurrent(out void* sharedGlContext);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_GlContext_Create__P_b(bool createThread, out void* statusOrSharedGlContext);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_GlContext_Create__Rgc_b(
            void* shareContext, bool createThread, out void* statusOrSharedGlContext);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_GlContext_Create__ui_b(
            uint shareContext, bool createThread, out void* statusOrSharedGlContext);

        [SupportedOSPlatform("IOS")]
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_GlContext_Create__Pes_b(
            void* sharegroup, bool createThread, out void* statusOrSharedGlContext);
        #endregion

        #region GlSyncToken
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void mp_GlSyncToken__delete(void* glSyncToken);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void mp_GlSyncToken__reset(void* glSyncToken);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_GlSyncPoint__Wait(void* glSyncPoint);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_GlSyncPoint__WaitOnGpu(void* glSyncPoint);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_GlSyncPoint__IsReady(void* glSyncPoint, out bool value);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_GlSyncPoint__GetContext(void* glSyncPoint, out void* sharedGlContext);
        #endregion
    }
}
