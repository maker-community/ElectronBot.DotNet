// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Runtime.InteropServices;

namespace Mediapipe.Net.Graphs.InstantMotionTracking
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Anchor3d
    {
        public float X;
        public float Y;
        public float Z;
        public int StickerId;

        public override string ToString()
        {
            return $"({X}, {Y}, {Z}), #{StickerId}";
        }
    }
}
