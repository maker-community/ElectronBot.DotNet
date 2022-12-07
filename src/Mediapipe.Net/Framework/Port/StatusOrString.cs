// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using Mediapipe.Net.Native;
using Mediapipe.Net.Util;

namespace Mediapipe.Net.Framework.Port
{
    public unsafe class StatusOrString : StatusOr<string>
    {
        public StatusOrString(void* ptr) : base(ptr) { }

        protected override void DeleteMpPtr()
        {
            UnsafeNativeMethods.mp_StatusOrString__delete(Ptr);
        }

        private Status? status;
        public override Status Status
        {
            get
            {
                if (status == null || status.IsDisposed)
                {
                    UnsafeNativeMethods.mp_StatusOrString__status(MpPtr, out var statusPtr).Assert();

                    GC.KeepAlive(this);
                    status = new Status(statusPtr);
                }
                return status;
            }
        }

        public override bool Ok() => SafeNativeMethods.mp_StatusOrString__ok(MpPtr) > 0;

        public override string? Value()
        {
            var str = MarshalStringFromNative(UnsafeNativeMethods.mp_StatusOrString__value);
            Dispose(); // respect move semantics

            return str;
        }

        public byte[] ValueAsByteArray()
        {
            UnsafeNativeMethods.mp_StatusOrString__bytearray(MpPtr, out var strPtr, out var size).Assert();
            GC.KeepAlive(this);

            byte[] bytes = UnsafeUtil.SafeArrayCopy((byte*)strPtr, size);
            UnsafeNativeMethods.delete_array__PKc(strPtr);
            Dispose();

            return bytes;
        }
    }
}
