// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Collections.Generic;
using Mediapipe.Net.Framework;
using Mediapipe.Net.Framework.Format;
using Mediapipe.Net.Framework.Packets;

namespace Mediapipe.Net.Solutions
{
    /// <summary>
    /// Computer Vision solution running on the CPU.
    /// </summary>
    /// <typeparam name="TPacket">The type of packet the calculator returns the secondary output in.</typeparam>
    /// <typeparam name="T">The type of secondary output.</typeparam>
    public abstract class CpuSolution : Solution
    {
        public readonly string ImageFrameInput;

        protected CpuSolution(string graphPath, string imageFrameInput, IDictionary<string, PacketType> outputs, SidePackets? sidePackets) : base(graphPath, outputs, sidePackets)
        {
            ImageFrameInput = imageFrameInput;
        }

        protected override IDictionary<string, object?> ProcessFrame(ImageFrame frame)
        {
            Dictionary<string, Packet> inputs = new();
            Timestamp timestamp = new Timestamp(SimulatedTimestamp);
            Packet packet = PacketFactory.ImageFramePacket(frame, timestamp);
            inputs.Add(ImageFrameInput, packet);

            return Process(inputs);
        }
    }
}
