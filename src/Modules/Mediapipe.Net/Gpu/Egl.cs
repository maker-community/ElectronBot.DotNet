// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Runtime.Versioning;
using Mediapipe.Net.Native;

namespace Mediapipe.Net.Gpu
{
    [SupportedOSPlatform("Linux"), SupportedOSPlatform("Android")]
    public unsafe class Egl
    {
        public static void* GetCurrentContext() => SafeNativeMethods.eglGetCurrentContext();
    }
}
