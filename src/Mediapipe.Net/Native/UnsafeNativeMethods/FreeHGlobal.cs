// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using System.Runtime.InteropServices;

namespace Mediapipe.Net.Native
{
    internal unsafe partial class UnsafeNativeMethods : NativeMethods
    {
        // This is required to be a field in order to bypass the GC.
        private static readonly FreeHGlobalDelegate freeHGlobalDelegate;

        static UnsafeNativeMethods()
        {
            freeHGlobalDelegate = new FreeHGlobalDelegate(freeHGlobal);
            mp_api__SetFreeHGlobal(freeHGlobalDelegate);
        }

        private delegate void FreeHGlobalDelegate(IntPtr hglobal);

        private static void freeHGlobal(IntPtr hglobal)
        {
            Marshal.FreeHGlobal(hglobal);
        }

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        private static extern void mp_api__SetFreeHGlobal(FreeHGlobalDelegate freeHGlobal);
    }
}
