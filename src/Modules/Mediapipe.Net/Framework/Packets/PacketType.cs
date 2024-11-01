// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

namespace Mediapipe.Net.Framework.Packets
{
    public enum PacketType
    {
        Bool,
        Int,
        Float,
        FloatArray,
        String,
        StringAsByteArray,
        ImageFrame,
        Anchor3dVector,
        GpuBuffer,
        ClassificationList,
        ClassificationListVector,
        Detection,
        DetectionVector,
        FaceGeometry,
        FaceGeometryVector,
        FrameAnnotation,
        LandmarkList,
        LandmarkListVector,
        NormalizedLandmarkList,
        NormalizedLandmarkListVector,
        Rect,
        RectVector,
        NormalizedRect,
        NormalizedRectVector,
        TimedModelMatrixProtoList
    }
}
