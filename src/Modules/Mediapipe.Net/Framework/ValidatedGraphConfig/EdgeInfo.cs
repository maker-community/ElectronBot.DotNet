// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Runtime.InteropServices;

namespace Mediapipe.Net.Framework.ValidatedGraphConfig
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct EdgeInfo
    {
        public readonly int Upstream;
        public readonly NodeRef ParentNode;
        public readonly string? Name;
        public readonly bool BackEdge;

        internal EdgeInfo(int upstream, NodeRef parentNode, string? name, bool backEdge)
        {
            Upstream = upstream;
            ParentNode = parentNode;
            Name = name;
            BackEdge = backEdge;
        }
    }
}
