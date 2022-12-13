// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using Mediapipe.Net.Framework.Protobuf;
using Mediapipe.Net.Native;

namespace Mediapipe.Net.Gpu
{
    public static class GpuBufferFormatExtension
    {
        public static ImageFormat.Types.Format ImageFormatFor(this GpuBufferFormat gpuBufferFormat)
            => SafeNativeMethods.mp__ImageFormatForGpuBufferFormat__ui(gpuBufferFormat);

        public static GlTextureInfo GlTextureInfoFor(this GpuBufferFormat gpuBufferFormat, int plane, GlVersion glVersion = GlVersion.KGles3)
        {
            UnsafeNativeMethods.mp__GlTextureInfoForGpuBufferFormat__ui_i_ui(gpuBufferFormat, plane, glVersion, out var glTextureInfo).Assert();
            return glTextureInfo;
        }
    }
}
