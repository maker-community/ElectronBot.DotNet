// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using Mediapipe.Net.Framework.Format;
using Mediapipe.Net.Native;

namespace Mediapipe.Net.Framework.Port
{
    public unsafe class StatusOrImageFrame : StatusOr<ImageFrame>
    {
        public StatusOrImageFrame(void* ptr) : base(ptr) { }

        protected override void DeleteMpPtr()
        {
            UnsafeNativeMethods.mp_StatusOrImageFrame__delete(Ptr);
        }

        private Status? status;
        public override Status Status
        {
            get
            {
                if (status == null || status.IsDisposed)
                {
                    UnsafeNativeMethods.mp_StatusOrImageFrame__status(MpPtr, out var statusPtr).Assert();

                    GC.KeepAlive(this);
                    status = new Status(statusPtr);
                }
                return status;
            }
        }

        public override bool Ok() => SafeNativeMethods.mp_StatusOrImageFrame__ok(MpPtr) > 0;

        public override ImageFrame Value()
        {
            UnsafeNativeMethods.mp_StatusOrImageFrame__value(MpPtr, out var imageFramePtr).Assert();
            Dispose();

            return new ImageFrame(imageFramePtr);
        }
    }
}
