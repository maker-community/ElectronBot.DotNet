// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using Mediapipe.Net.Framework.Format;

namespace Mediapipe.Net.Solutions
{
    public interface IComputingSolution<T> : IDisposable
    {
        T Compute(ImageFrame image);
    }
}
