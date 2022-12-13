// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using Mediapipe.Net.Core;
using Mediapipe.Net.Native;

namespace Mediapipe.Net.Gpu
{
    public unsafe class GlTextureBuffer : MpResourceHandle
    {
        private SharedPtrHandle? sharedPtrHandle;

        /// <remarks>
        ///   In the original MediaPipe repo, DeletionCallback only receives GlSyncToken.
        ///   However, IL2CPP does not support marshaling delegates that point to instance methods to native code,
        ///   so it receives also the texture name to specify the target instance.
        /// </remarks>
        public delegate void DeletionCallback(uint name, void* glSyncToken);

        public GlTextureBuffer(void* ptr, bool isOwner = true) : base(isOwner)
        {
            sharedPtrHandle = new SharedGlTextureBufferPtr(ptr, isOwner);
            Ptr = sharedPtrHandle.Get();
        }

        /// <param name="callback">
        ///   A function called when the texture buffer is deleted.
        ///   Make sure that this function doesn't throw exceptions and won't be GCed.
        /// </param>
        public GlTextureBuffer(uint target, uint name, int width, int height,
            GpuBufferFormat format, DeletionCallback callback, GlContext? glContext) : base()
        {
            var sharedContextPtr = glContext == null ? null : glContext.SharedPtr;
            UnsafeNativeMethods.mp_SharedGlTextureBuffer__ui_ui_i_i_ui_PF_PSgc(
                target, name, width, height, format, callback, sharedContextPtr, out var ptr).Assert();

            sharedPtrHandle = new SharedGlTextureBufferPtr(ptr);
            Ptr = sharedPtrHandle.Get();
        }

        public GlTextureBuffer(uint name, int width, int height, GpuBufferFormat format, DeletionCallback callback, GlContext? glContext = null) :
            this(Gl.GL_TEXTURE_2D, name, width, height, format, callback, glContext)
        { }

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

        public uint Name() => SafeNativeMethods.mp_GlTextureBuffer__name(MpPtr);

        public uint Target() => SafeNativeMethods.mp_GlTextureBuffer__target(MpPtr);

        public int Width() => SafeNativeMethods.mp_GlTextureBuffer__width(MpPtr);

        public int Height() => SafeNativeMethods.mp_GlTextureBuffer__height(MpPtr);

        public GpuBufferFormat Format => SafeNativeMethods.mp_GlTextureBuffer__format(MpPtr);

        public void WaitUntilComplete() => UnsafeNativeMethods.mp_GlTextureBuffer__WaitUntilComplete(MpPtr).Assert();

        public void WaitOnGpu() => UnsafeNativeMethods.mp_GlTextureBuffer__WaitOnGpu(MpPtr).Assert();

        public void Reuse() => UnsafeNativeMethods.mp_GlTextureBuffer__Reuse(MpPtr).Assert();

        public void Updated(GlSyncPoint prodToken) => UnsafeNativeMethods.mp_GlTextureBuffer__Updated__Pgst(MpPtr, prodToken.SharedPtr).Assert();

        public void DidRead(GlSyncPoint consToken) => UnsafeNativeMethods.mp_GlTextureBuffer__DidRead__Pgst(MpPtr, consToken.SharedPtr).Assert();

        public void WaitForConsumers() => UnsafeNativeMethods.mp_GlTextureBuffer__WaitForConsumers(MpPtr).Assert();

        public void WaitForConsumersOnGpu() => UnsafeNativeMethods.mp_GlTextureBuffer__WaitForConsumersOnGpu(MpPtr).Assert();

        public GlContext GetProducerContext() => new GlContext(SafeNativeMethods.mp_GlTextureBuffer__GetProducerContext(MpPtr), false);

        // TODO: Put it in its own file
        private class SharedGlTextureBufferPtr : SharedPtrHandle
        {
            public SharedGlTextureBufferPtr(void* ptr, bool isOwner = true) : base(ptr, isOwner) { }

            protected override void DeleteMpPtr() => UnsafeNativeMethods.mp_SharedGlTextureBuffer__delete(Ptr);

            public override void* Get() => SafeNativeMethods.mp_SharedGlTextureBuffer__get(MpPtr);

            public override void Reset() => UnsafeNativeMethods.mp_SharedGlTextureBuffer__reset(MpPtr);
        }
    }
}
