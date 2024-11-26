// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using Mediapipe.Net.Core;
using Mediapipe.Net.Native;

namespace Mediapipe.Net.External
{
    public unsafe class StdString : MpResourceHandle
    {
        public StdString(void* ptr, bool isOwner = true) : base(ptr, isOwner) { }

        public StdString(byte[] bytes) : base()
        {
            UnsafeNativeMethods.std_string__PKc_i(bytes, bytes.Length, out var ptr).Assert();
            Ptr = ptr;
        }

        protected override void DeleteMpPtr() => UnsafeNativeMethods.std_string__delete((sbyte*)Ptr);

        public void Swap(StdString str)
        {
            UnsafeNativeMethods.std_string__swap__Rstr(MpPtr, str.MpPtr);
            GC.KeepAlive(this);
        }
    }
}
