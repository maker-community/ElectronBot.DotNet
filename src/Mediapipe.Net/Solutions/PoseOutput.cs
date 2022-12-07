// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using Mediapipe.Net.Framework.Format;
using Mediapipe.Net.Framework.Protobuf;

namespace Mediapipe.Net.Solutions
{
    public class PoseOutput
    {
        public NormalizedLandmarkList? PoseLandmarks { get; set; }
        public LandmarkList? PoseWorldLandmarks { get; set; }
        public ImageFrame? SegmentationMask { get; set; }
    }
}
