// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Collections.Generic;
using Mediapipe.Net.Framework.Protobuf;

namespace Mediapipe.Net.Solutions
{
    public class HandsOutput
    {
        public List<NormalizedLandmarkList>? MultiHandLandmarks { get; set; }
        public List<LandmarkList>? MultiHandWorldLandmarks { get; set; }
        public List<ClassificationList>? MultiHandedness { get; set; }
    }
}
