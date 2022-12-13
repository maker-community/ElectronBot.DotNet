// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using Mediapipe.Net.Native;

namespace Mediapipe.Net.External
{
    public static class Glog
    {
        public enum Severity : int
        {
            Info = 0,
            Warning = 1,
            Error = 2,
            Fatal = 3,
        }

        private static bool logToStderr = false;
        public static bool LogToStderr
        {
            get => logToStderr;
            set
            {
                UnsafeNativeMethods.glog_FLAGS_logtostderr(value);
                logToStderr = value;
            }
        }

        private static int stderrThreshold = 2;
        public static int StderrThreshold
        {
            get => stderrThreshold;
            set
            {
                UnsafeNativeMethods.glog_FLAGS_stderrthreshold(value);
                stderrThreshold = value;
            }
        }

        private static int minLogLevel = 0;
        public static int MinLogLevel
        {
            get => minLogLevel;
            set
            {
                UnsafeNativeMethods.glog_FLAGS_minloglevel(value);
                minLogLevel = value;
            }
        }

        private static string? logDir;
        public static string? LogDir
        {
            get => logDir;
            set
            {
                UnsafeNativeMethods.glog_FLAGS_log_dir(value ?? "");
                logDir = value;
            }
        }

        private static int v = 0;
        public static int V
        {
            get => v;
            set
            {
                UnsafeNativeMethods.glog_FLAGS_v(value);
                v = value;
            }
        }

        public static void Initialize(string name)
            => UnsafeNativeMethods.google_InitGoogleLogging__PKc(name).Assert();

        public static void Shutdown()
            => UnsafeNativeMethods.google_ShutdownGoogleLogging().Assert();

        public static void Log(Severity severity, string str)
        {
            switch (severity)
            {
                case Severity.Info:
                    UnsafeNativeMethods.glog_LOG_INFO__PKc(str);
                    break;
                case Severity.Warning:
                    UnsafeNativeMethods.glog_LOG_WARNING__PKc(str);
                    break;
                case Severity.Error:
                    UnsafeNativeMethods.glog_LOG_ERROR__PKc(str);
                    break;
                case Severity.Fatal:
                    UnsafeNativeMethods.glog_LOG_FATAL__PKc(str);
                    break;
                default:
                    throw new ArgumentException($"Unknown Severity: {severity}");
            }
        }

        public static void FlushLogFiles(Severity severity = Severity.Info)
            => UnsafeNativeMethods.google_FlushLogFiles(severity);
    }
}
