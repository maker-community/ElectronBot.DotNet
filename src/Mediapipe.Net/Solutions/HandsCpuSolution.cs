// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Collections.Generic;
using Mediapipe.Net.Framework.Format;
using Mediapipe.Net.Framework.Packets;
using Mediapipe.Net.Framework.Protobuf;

namespace Mediapipe.Net.Solutions
{
    /// <summary>
    /// Hands solution running on the CPU.
    /// </summary>
    /// <typeparam name="TPacket">The type of packet the calculator returns the secondary output in.</typeparam>
    /// <typeparam name="T">The type of secondary output.</typeparam>
    public class HandsCpuSolution : CpuSolution, IComputingSolution<HandsOutput>
    {
        private static readonly string graphPath = "mediapipe/modules/hand_landmark/hand_landmark_tracking_cpu.pbtxt";

        private static readonly IDictionary<string, PacketType> outputs = new Dictionary<string, PacketType>()
        {
            { "multi_hand_landmarks", PacketType.NormalizedLandmarkListVector },
            { "multi_hand_world_landmarks", PacketType.LandmarkListVector },
            { "multi_handedness", PacketType.ClassificationListVector },
        };

        public static readonly PacketType PacketType = PacketType.NormalizedLandmarkListVector;

        private static SidePackets toSidePackets(bool staticImageMode, int maxNumHands, int modelComplexity)
        {
            SidePackets sidePackets = new();
            sidePackets.Emplace("use_prev_landmarks", PacketFactory.BoolPacket(!staticImageMode));
            sidePackets.Emplace("num_hands", PacketFactory.IntPacket(maxNumHands));
            sidePackets.Emplace("model_complexity", PacketFactory.IntPacket(modelComplexity));
            return sidePackets;
        }

        public HandsCpuSolution(
            bool staticImageMode = false,
            int maxNumHands = 2,
            int modelComplexity = 1
        ) : base(graphPath, "image", outputs, toSidePackets(staticImageMode, maxNumHands, modelComplexity))
        {
        }

        public HandsOutput Compute(ImageFrame frame)
        {
            IDictionary<string, object?> graphOutputs = ProcessFrame(frame);

            HandsOutput handsOutput = new();

            if (graphOutputs.ContainsKey("multi_hand_landmarks"))
                handsOutput.MultiHandLandmarks = (List<NormalizedLandmarkList>?)graphOutputs["multi_hand_landmarks"];

            if (graphOutputs.ContainsKey("multi_hand_world_landmarks"))
                handsOutput.MultiHandWorldLandmarks = (List<LandmarkList>?)graphOutputs["multi_hand_world_landmarks"];

            if (graphOutputs.ContainsKey("multi_handedness"))
                handsOutput.MultiHandedness = (List<ClassificationList>?)graphOutputs["multi_handedness"];

            return handsOutput;
        }
    }
}
