// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

namespace Mediapipe.Net.Core
{
    public unsafe abstract class UniquePtrHandle : MpResourceHandle
    {
        protected UniquePtrHandle(void* ptr, bool isOwner = true) : base(ptr, isOwner) { }

        /// <returns>The owning pointer</returns>
        public abstract void* Get();

        /// <summary>Release the owning pointer</summary>
        public abstract void* Release();
    }
}
