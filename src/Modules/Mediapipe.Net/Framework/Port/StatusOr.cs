// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using Mediapipe.Net.Core;

namespace Mediapipe.Net.Framework.Port
{
    public unsafe abstract class StatusOr<T> : MpResourceHandle
    {
        public StatusOr(void* ptr) : base(ptr) { }

        public abstract Status Status { get; }
        public virtual bool Ok() => Status.Ok();

        public virtual T? ValueOr(T? defaultValue = default) => Ok() ? Value() : defaultValue;

        /// <exception cref="MediapipeNetException">Thrown when status is not ok</exception>
        public abstract T? Value();
    }
}
