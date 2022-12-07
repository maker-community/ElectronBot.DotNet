// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Runtime.InteropServices;

namespace Mediapipe.Net.Native
{
    internal unsafe partial class UnsafeNativeMethods : NativeMethods
    {
        [DllImport(MEDIAPIPE_LIBRARY)]
        public static extern void glFlush();

        [DllImport(MEDIAPIPE_LIBRARY)]
        public static extern void glReadPixels(int x, int y, int width, int height, uint glFormat, uint glType, void* pixels);
    }
}
