// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Runtime.InteropServices;

namespace Mediapipe.Net.Gpu
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GlTextureInfo
    {
        public int GlInternalFormat;
        public uint GlFormat;
        public uint GlType;
        public int Downscale;
    }
}
