// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

namespace Mediapipe.Net.Gpu
{
    public enum GpuBufferFormat : uint
    {
        KUnknown = 0,
        KBgra32 = ('B' << 24) + ('G' << 16) + ('R' << 8) + 'A',
        kGrayFloat32 = ('L' << 24) + ('0' << 16) + ('0' << 8) + 'f',
        KGrayHalf16 = ('L' << 24) + ('0' << 16) + ('0' << 8) + 'h',
        KOneComponent8 = ('L' << 24) + ('0' << 16) + ('0' << 8) + '8',
        KTwoComponentHalf16 = ('2' << 24) + ('C' << 16) + ('0' << 8) + 'h',
        KTwoComponentFloat32 = ('2' << 24) + ('C' << 16) + ('0' << 8) + 'f',
        KBiPlanar420YpCbCr8VideoRange = ('4' << 24) + ('2' << 16) + ('0' << 8) + 'v',
        KBiPlanar420YpCbCr8FullRange = ('4' << 24) + ('2' << 16) + ('0' << 8) + 'f',
        KRgb24 = 0x00000018,  // Note: prefer Bgra32 whenever possible.
        KRgbaHalf64 = ('R' << 24) + ('G' << 16) + ('h' << 8) + 'A',
        KRgbaFloat128 = ('R' << 24) + ('G' << 16) + ('f' << 8) + 'A',
    }
}
