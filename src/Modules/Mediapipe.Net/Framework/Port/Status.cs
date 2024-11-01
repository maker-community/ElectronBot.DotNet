// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using System.Runtime.InteropServices;
using Mediapipe.Net.Core;
using Mediapipe.Net.Native;

namespace Mediapipe.Net.Framework.Port
{
    public unsafe class Status : MpResourceHandle
    {
        public enum StatusCode : int
        {
            Ok = 0,
            Cancelled = 1,
            Unknown = 2,
            InvalidArgument = 3,
            DeadlineExceeded = 4,
            NotFound = 5,
            AlreadyExists = 6,
            PermissionDenied = 7,
            ResourceExhausted = 8,
            FailedPrecondition = 9,
            Aborted = 10,
            OutOfRange = 11,
            Unimplemented = 12,
            Internal = 13,
            Unavailable = 14,
            DataLoss = 15,
            Unauthenticated = 16,
        }

        [StructLayout(LayoutKind.Sequential)]
        public readonly struct StatusArgs
        {
            private readonly StatusCode code;
            private readonly IntPtr message;

            private StatusArgs(StatusCode code, string? message = null)
            {
                this.code = code;
                this.message = Marshal.StringToHGlobalAnsi(message);
            }

            public static StatusArgs Ok() => new StatusArgs(StatusCode.Ok);

            public static StatusArgs Cancelled(string? message = null)
                => new StatusArgs(StatusCode.Cancelled, message);

            public static StatusArgs Unknown(string? message = null)
                => new StatusArgs(StatusCode.Unknown, message);

            public static StatusArgs InvalidArgument(string? message = null)
                => new StatusArgs(StatusCode.InvalidArgument, message);

            public static StatusArgs DeadlineExceeded(string? message = null)
                => new StatusArgs(StatusCode.DeadlineExceeded, message);

            public static StatusArgs NotFound(string? message = null)
                => new StatusArgs(StatusCode.NotFound, message);

            public static StatusArgs AlreadyExists(string? message = null)
                => new StatusArgs(StatusCode.AlreadyExists, message);

            public static StatusArgs PermissionDenied(string? message = null)
                => new StatusArgs(StatusCode.PermissionDenied, message);

            public static StatusArgs ResourceExhausted(string? message = null)
                => new StatusArgs(StatusCode.ResourceExhausted, message);

            public static StatusArgs FailedPrecondition(string? message = null)
                => new StatusArgs(StatusCode.FailedPrecondition, message);

            public static StatusArgs Aborted(string? message = null)
                => new StatusArgs(StatusCode.Aborted, message);

            public static StatusArgs OutOfRange(string? message = null)
                => new StatusArgs(StatusCode.OutOfRange, message);

            public static StatusArgs Unimplemented(string? message = null)
                => new StatusArgs(StatusCode.Unimplemented, message);

            public static StatusArgs Internal(string? message = null)
                => new StatusArgs(StatusCode.Internal, message);

            public static StatusArgs Unavailable(string? message = null)
                => new StatusArgs(StatusCode.Unavailable, message);

            public static StatusArgs DataLoss(string? message = null)
                => new StatusArgs(StatusCode.DataLoss, message);

            public static StatusArgs Unauthenticated(string? message = null)
                => new StatusArgs(StatusCode.Unauthenticated, message);
        }

        public Status(void* ptr, bool isOwner = true) : base(ptr, isOwner) { }

        protected override void DeleteMpPtr() => UnsafeNativeMethods.absl_Status__delete(Ptr);

        private bool? ok;
        private int? rawCode;

        public void AssertOk()
        {
            if (!Ok())
                throw new MediapipeException(ToString() ?? "");
        }

        public bool Ok()
        {
            if (ok is bool valueOfOk)
                return valueOfOk;
            ok = SafeNativeMethods.absl_Status__ok(MpPtr) > 0;
            return (bool)ok;
        }

        public StatusCode Code => (StatusCode)RawCode;

        public int RawCode
        {
            get
            {
                if (rawCode is int valueOfRawCode)
                    return valueOfRawCode;

                rawCode = SafeNativeMethods.absl_Status__raw_code(MpPtr);
                return (int)rawCode;
            }
        }

        public override string? ToString() => MarshalStringFromNative(UnsafeNativeMethods.absl_Status__ToString);

        public static Status Build(StatusCode code, string message, bool isOwner = true)
        {
            UnsafeNativeMethods.absl_Status__i_PKc((int)code, message, out var ptr).Assert();

            return new Status(ptr, isOwner);
        }

        public static Status Ok(bool isOwner = true) => Build(StatusCode.Ok, "", isOwner);

        public static Status Cancelled(string message = "", bool isOwner = true)
            => Build(StatusCode.Cancelled, message, isOwner);

        public static Status Unknown(string message = "", bool isOwner = true)
            => Build(StatusCode.Unknown, message, isOwner);

        public static Status InvalidArgument(string message = "", bool isOwner = true)
            => Build(StatusCode.InvalidArgument, message, isOwner);

        public static Status DeadlineExceeded(string message = "", bool isOwner = true)
            => Build(StatusCode.DeadlineExceeded, message, isOwner);

        public static Status NotFound(string message = "", bool isOwner = true)
            => Build(StatusCode.NotFound, message, isOwner);

        public static Status AlreadyExists(string message = "", bool isOwner = true)
            => Build(StatusCode.AlreadyExists, message, isOwner);

        public static Status PermissionDenied(string message = "", bool isOwner = true)
            => Build(StatusCode.PermissionDenied, message, isOwner);

        public static Status ResourceExhausted(string message = "", bool isOwner = true)
            => Build(StatusCode.ResourceExhausted, message, isOwner);

        public static Status FailedPrecondition(string message = "", bool isOwner = true)
            => Build(StatusCode.FailedPrecondition, message, isOwner);

        public static Status Aborted(string message = "", bool isOwner = true)
            => Build(StatusCode.Aborted, message, isOwner);

        public static Status OutOfRange(string message = "", bool isOwner = true)
            => Build(StatusCode.OutOfRange, message, isOwner);

        public static Status Unimplemented(string message = "", bool isOwner = true)
            => Build(StatusCode.Unimplemented, message, isOwner);

        public static Status Internal(string message = "", bool isOwner = true)
            => Build(StatusCode.Internal, message, isOwner);

        public static Status Unavailable(string message = "", bool isOwner = true)
            => Build(StatusCode.Unavailable, message, isOwner);

        public static Status DataLoss(string message = "", bool isOwner = true)
            => Build(StatusCode.DataLoss, message, isOwner);

        public static Status Unauthenticated(string message = "", bool isOwner = true)
            => Build(StatusCode.Unauthenticated, message, isOwner);
    }
}
