// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Runtime.Versioning;
using Mediapipe.Net.Core;
using Mediapipe.Net.Native;

namespace Mediapipe.Net.Gpu
{
    public unsafe class GlContext : MpResourceHandle
    {
        private SharedPtrHandle? sharedPtrHandle;

        public static GlContext? GetCurrent()
        {
            UnsafeNativeMethods.mp_GlContext_GetCurrent(out var glContextPtr).Assert();
            return glContextPtr == null ? null : new GlContext(glContextPtr);
        }

        public GlContext(void* ptr, bool isOwner = true) : base(isOwner)
        {
            sharedPtrHandle = new SharedGlContextPtr(ptr, isOwner);
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

        [SupportedOSPlatform("Linux"), SupportedOSPlatform("Android")]
        public void* EglDisplay => SafeNativeMethods.mp_GlContext__egl_display(MpPtr);

        [SupportedOSPlatform("Linux"), SupportedOSPlatform("Android")]
        public void* EglConfig => SafeNativeMethods.mp_GlContext__egl_config(MpPtr);

        [SupportedOSPlatform("Linux"), SupportedOSPlatform("Android")]
        public void* EglContext => SafeNativeMethods.mp_GlContext__egl_context(MpPtr);

        // NOTE: (from homuler) On macOS, native libs cannot be built with GPU enabled, so it cannot be used actually.
        [SupportedOSPlatform("OSX")]
        public void* NsglContext => SafeNativeMethods.mp_GlContext__nsgl_context(MpPtr);

        [SupportedOSPlatform("IOS")]
        public void* EaglContext => SafeNativeMethods.mp_GlContext__eagl_context(MpPtr);

        public bool IsCurrent() => SafeNativeMethods.mp_GlContext__IsCurrent(MpPtr) > 0;

        public int GlMajorVersion => SafeNativeMethods.mp_GlContext__gl_major_version(MpPtr);

        public int GlMinorVersion => SafeNativeMethods.mp_GlContext__gl_minor_version(MpPtr);

        public long GlFinishCount => SafeNativeMethods.mp_GlContext__gl_finish_count(MpPtr);

        // TODO: Put it in its own file
        private class SharedGlContextPtr : SharedPtrHandle
        {
            public SharedGlContextPtr(void* ptr, bool isOwner = true) : base(ptr, isOwner) { }

            protected override void DeleteMpPtr() => UnsafeNativeMethods.mp_SharedGlContext__delete(Ptr);

            public override void* Get() => SafeNativeMethods.mp_SharedGlContext__get(MpPtr);

            public override void Reset() => UnsafeNativeMethods.mp_SharedGlContext__reset(MpPtr);
        }
    }
}
