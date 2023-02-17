// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using Mediapipe.Net.Core;
using Mediapipe.Net.Native;

namespace Mediapipe.Net.Gpu
{
    public unsafe class GlSyncPoint : MpResourceHandle
    {
        private SharedPtrHandle? sharedPtrHandle;

        public GlSyncPoint(void* ptr) : base()
        {
            sharedPtrHandle = new SharedGlSyncPointPtr(ptr);
            Ptr = sharedPtrHandle.Get();
        }

        protected override void DisposeManaged()
        {
            if (sharedPtrHandle != null)
            {
                sharedPtrHandle.Dispose();
                sharedPtrHandle = null;
            }
            base.DisposeManaged();
        }

        protected override void DeleteMpPtr()
        {
            // Do nothing
        }

        public void* SharedPtr => sharedPtrHandle == null ? null : sharedPtrHandle.MpPtr;

        public void Wait() => UnsafeNativeMethods.mp_GlSyncPoint__Wait(MpPtr).Assert();

        public void WaitOnGpu() => UnsafeNativeMethods.mp_GlSyncPoint__WaitOnGpu(MpPtr).Assert();

        public bool IsReady
        {
            get
            {
                UnsafeNativeMethods.mp_GlSyncPoint__IsReady(MpPtr, out var value).Assert();

                GC.KeepAlive(this);
                return value;
            }
        }
        public GlContext GetContext()
        {
            UnsafeNativeMethods.mp_GlSyncPoint__GetContext(MpPtr, out var sharedGlContextPtr).Assert();

            GC.KeepAlive(this);
            return new GlContext(sharedGlContextPtr);
        }

        // TODO: Put it in its own file
        private class SharedGlSyncPointPtr : SharedPtrHandle
        {
            public SharedGlSyncPointPtr(void* ptr) : base(ptr) { }

            protected override void DeleteMpPtr() => UnsafeNativeMethods.mp_GlSyncToken__delete(Ptr);

            public override void* Get() => SafeNativeMethods.mp_GlSyncToken__get(MpPtr);

            public override void Reset() => UnsafeNativeMethods.mp_GlSyncToken__reset(MpPtr);
        }
    }
}
