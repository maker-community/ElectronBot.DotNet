// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Runtime.InteropServices;

namespace Mediapipe.Net.Native
{
    internal unsafe partial class UnsafeNativeMethods : NativeMethods
    {
        #region OutputStreamPoller
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void mp_OutputStreamPoller__delete(void* poller);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_OutputStreamPoller__Reset(void* poller);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_OutputStreamPoller__Next_Ppacket(void* poller, void* packet, out bool result);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_OutputStreamPoller__SetMaxQueueSize(void* poller, int queueSize);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_OutputStreamPoller__QueueSize(void* poller, out int queueSize);
        #endregion

        #region StatusOrPoller
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void mp_StatusOrPoller__delete(void* statusOrPoller);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_StatusOrPoller__status(void* statusOrPoller, out void* status);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_StatusOrPoller__value(void* statusOrPoller, out void* poller);
        #endregion
    }
}
