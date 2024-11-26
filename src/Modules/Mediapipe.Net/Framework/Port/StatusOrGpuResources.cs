// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using Mediapipe.Net.Gpu;
using Mediapipe.Net.Native;

namespace Mediapipe.Net.Framework.Port
{
    public unsafe class StatusOrGpuResources : StatusOr<GpuResources>
    {
        public StatusOrGpuResources(void* ptr) : base(ptr) { }

        protected override void DeleteMpPtr()
        {
            UnsafeNativeMethods.mp_StatusOrGpuResources__delete(Ptr);
        }

        private Status? status;
        public override Status Status
        {
            get
            {
                if (status == null || status.IsDisposed)
                {
                    UnsafeNativeMethods.mp_StatusOrGpuResources__status(MpPtr, out var statusPtr).Assert();

                    GC.KeepAlive(this);
                    status = new Status(statusPtr);
                }
                return status;
            }
        }

        public override bool Ok() => SafeNativeMethods.mp_StatusOrGpuResources__ok(MpPtr) > 0;

        public override GpuResources Value()
        {
            UnsafeNativeMethods.mp_StatusOrGpuResources__value(MpPtr, out var gpuResourcesPtr).Assert();
            Dispose();

            return new GpuResources(gpuResourcesPtr);
        }
    }
}
