// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

namespace Mediapipe.Net.Framework.ValidatedGraphConfig
{
    public enum NodeType : int
    {
        Unknown = 0,
        Calculator = 1,
        PacketGenerator = 2,
        GraphInputStream = 3,
        StatusHandler = 4,
    };
}
