// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Runtime.Versioning;
using Mediapipe.Net.Core;
using Mediapipe.Net.Framework.Port;
using Mediapipe.Net.Native;

namespace Mediapipe.Net.Gpu
{
    public unsafe class GpuResources : MpResourceHandle
    {
        private SharedPtrHandle? sharedPtrHandle;

        /// <param name="ptr">Shared pointer of mediapipe::GpuResources</param>
        public GpuResources(void* ptr) : base()
        {
            sharedPtrHandle = new SharedGpuResourcesPtr(ptr);
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

        public static StatusOrGpuResources Create()
        {
            UnsafeNativeMethods.mp_GpuResources_Create(out var statusOrGpuResourcesPtr).Assert();

            return new StatusOrGpuResources(statusOrGpuResourcesPtr);
        }

        public static StatusOrGpuResources Create(void* externalContext)
        {
            UnsafeNativeMethods.mp_GpuResources_Create__Pv(externalContext, out var statusOrGpuResourcesPtr).Assert();

            return new StatusOrGpuResources(statusOrGpuResourcesPtr);
        }

        [SupportedOSPlatform("IOS")]
        public void* IosGpuData => SafeNativeMethods.mp_GpuResources__ios_gpu_data(MpPtr);

        private class SharedGpuResourcesPtr : SharedPtrHandle
        {
            public SharedGpuResourcesPtr(void* ptr) : base(ptr) { }

            protected override void DeleteMpPtr() => UnsafeNativeMethods.mp_SharedGpuResources__delete(Ptr);

            public override void* Get() => SafeNativeMethods.mp_SharedGpuResources__get(MpPtr);

            public override void Reset() => UnsafeNativeMethods.mp_SharedGpuResources__reset(MpPtr);
        }
    }
}
