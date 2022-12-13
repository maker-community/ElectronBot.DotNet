// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using Mediapipe.Net.Core;
using Mediapipe.Net.Native;

namespace Mediapipe.Net.Gpu
{
    public unsafe class GlTexture : MpResourceHandle
    {
        public GlTexture() : base()
        {
            UnsafeNativeMethods.mp_GlTexture__(out var ptr).Assert();
            Ptr = ptr;
        }

        public GlTexture(void* ptr, bool isOwner = true) : base(ptr, isOwner) { }

        protected override void DeleteMpPtr() => UnsafeNativeMethods.mp_GlTexture__delete(Ptr);

        public int Width => SafeNativeMethods.mp_GlTexture__width(MpPtr);

        public int Height => SafeNativeMethods.mp_GlTexture__height(MpPtr);

        public uint Target => SafeNativeMethods.mp_GlTexture__target(MpPtr);

        public uint Name => SafeNativeMethods.mp_GlTexture__name(MpPtr);

        public void Release()
        {
            UnsafeNativeMethods.mp_GlTexture__Release(MpPtr).Assert();
            GC.KeepAlive(this);
        }

        public GpuBuffer GetGpuBufferFrame()
        {
            UnsafeNativeMethods.mp_GlTexture__GetGpuBufferFrame(MpPtr, out var gpuBufferPtr).Assert();

            GC.KeepAlive(this);
            return new GpuBuffer(gpuBufferPtr);
        }
    }
}
