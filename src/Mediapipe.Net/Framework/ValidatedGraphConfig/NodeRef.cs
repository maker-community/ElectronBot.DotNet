// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Runtime.InteropServices;

namespace Mediapipe.Net.Framework.ValidatedGraphConfig
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct NodeRef
    {
        public readonly NodeType Type;
        public readonly int Index;
    }
}
