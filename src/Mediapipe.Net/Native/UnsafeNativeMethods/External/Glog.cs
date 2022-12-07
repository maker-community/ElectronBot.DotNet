// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Runtime.InteropServices;
using Mediapipe.Net.External;

namespace Mediapipe.Net.Native
{
    internal unsafe partial class UnsafeNativeMethods : NativeMethods
    {
#pragma warning disable CA2101
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern MpReturnCode google_InitGoogleLogging__PKc(string name);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode google_ShutdownGoogleLogging();

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void glog_FLAGS_logtostderr(bool value);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void glog_FLAGS_stderrthreshold(int threshold);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void glog_FLAGS_minloglevel(int level);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern void glog_FLAGS_log_dir(string dir);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void glog_FLAGS_v(int v);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern void glog_LOG_INFO__PKc(string str);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern void glog_LOG_WARNING__PKc(string str);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern void glog_LOG_ERROR__PKc(string str);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern void glog_LOG_FATAL__PKc(string str);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void google_FlushLogFiles(Glog.Severity severity);
#pragma warning restore CA2101
    }
}
