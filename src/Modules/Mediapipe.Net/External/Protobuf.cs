// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using Mediapipe.Net.Native;

namespace Mediapipe.Net.External
{
    public static class Protobuf
    {
        public delegate void LogHandler(int level, string filename, int line, string message);
        public static readonly LogHandler DefaultLogHandler = logProtobufMessage;

        public static void SetLogHandler(LogHandler logHandler)
        {
            UnsafeNativeMethods.google_protobuf__SetLogHandler__PF(logHandler).Assert();
        }

        /// <summary>
        ///   Reset the <see cref="LogHandler" />.
        ///   If <see cref="SetLogHandler" /> is called, this method should be called before the program exits.
        /// </summary>
        public static void ResetLogHandler()
        {
            UnsafeNativeMethods.google_protobuf__ResetLogHandler().Assert();
        }

        private static void logProtobufMessage(int level, string filename, int line, string message)
        {
            switch (level)
            {
                case 1:
                    Console.Error.WriteLine($"[libprotobuf WARN  {filename}:{line}] {message}");
                    break;
                case 2:
                    Console.Error.WriteLine($"[libprotobuf ERROR {filename}:{line}] {message}");
                    break;
                case 3:
                    Console.Error.WriteLine($"[libprotobuf FATAL {filename}:{line}] {message}");
                    break;
                default:
                    Console.Error.WriteLine($"[libprotobuf INFO  {filename}:{line}] {message}");
                    break;
            }
        }
    }
}
