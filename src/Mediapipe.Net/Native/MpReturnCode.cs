// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

namespace Mediapipe.Net.Native
{
    /// <summary>
    /// Code returned by most of MediaPipe's unsafe methods.
    /// </summary>
    public enum MpReturnCode : int
    {
        /// <summary>
        /// Nothing gone wrong.
        /// </summary>
        Success = 0,
        /// <summary>
        /// A standard exception is thrown.
        /// </summary>
        StandardError = 1,
        /// <summary>
        /// Something other than standard exception is thrown.
        /// </summary>
        UnknownError = 70,
        /// <summary>
        /// SDK failed to set status code (bug).
        /// </summary>
        Unset = 128,
        /// <summary>
        /// Received SIGABRT.
        /// </summary>
        Aborted = 134,
    }
}
