// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;

namespace Mediapipe.Net.Core
{
    public unsafe interface IMpResourceHandle : IDisposable
    {
        void* MpPtr { get; }

        /// <summary>
        /// Relinquish the ownership, and release the resource it owns if necessary.
        /// This method should be called only if the underlying native api moves the pointer.
        /// </summary>
        /// <remarks>
        /// If the object itself is no longer used, call <see cref="Dispose" /> instead.
        /// </remarks>
        void ReleaseMpResource();

        /// <summary>
        /// Relinquish the ownership
        /// </summary>
        void TransferOwnership();

        bool OwnsResource { get; }
    }
}
