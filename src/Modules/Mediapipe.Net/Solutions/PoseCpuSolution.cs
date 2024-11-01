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
    /// Face Mesh solution running on the CPU.
    /// </summary>
    /// <typeparam name="TPacket">The type of packet the calculator returns the secondary output in.</typeparam>
    /// <typeparam name="T">The type of secondary output.</typeparam>
    public class PoseCpuSolution : CpuSolution, IComputingSolution<PoseOutput>
    {
        private static readonly string graphPath = "mediapipe/modules/pose_landmark/pose_landmark_cpu.pbtxt";

        private static readonly IDictionary<string, PacketType> outputs = new Dictionary<string, PacketType>()
        {
            { "pose_landmarks", PacketType.NormalizedLandmarkList },
            { "pose_world_landmarks", PacketType.LandmarkList },
            { "segmentation_mask", PacketType.ImageFrame },
        };

        private static SidePackets toSidePackets(
            bool staticImageMode,
            int modelComplexity,
            bool smoothLandmarks,
            bool enableSegmentation,
            bool smoothSegmentation)
        {
            SidePackets sidePackets = new();
            sidePackets.Emplace("use_prev_landmarks", PacketFactory.BoolPacket(!staticImageMode));
            sidePackets.Emplace("model_complexity", PacketFactory.IntPacket(modelComplexity));
            sidePackets.Emplace("smooth_landmarks", PacketFactory.BoolPacket(smoothLandmarks && !staticImageMode));
            sidePackets.Emplace("enable_segmentation", PacketFactory.BoolPacket(enableSegmentation));
            sidePackets.Emplace("smooth_segmentation", PacketFactory.BoolPacket(smoothSegmentation && !staticImageMode));
            return sidePackets;
        }

        public PoseCpuSolution(
            bool staticImageMode = false,
            int modelComplexity = 1,
            bool smoothLandmarks = true,
            bool enableSegmentation = false,
            bool smoothSegmentation = true
        ) : base(graphPath, "image", outputs, toSidePackets(staticImageMode, modelComplexity, smoothLandmarks, enableSegmentation, smoothSegmentation))
        {
        }

        public PoseOutput Compute(ImageFrame frame)
        {
            IDictionary<string, object?> graphOutputs = ProcessFrame(frame);

            PoseOutput poseOutput = new();

            if (graphOutputs.ContainsKey("pose_landmarks"))
                poseOutput.PoseLandmarks = (NormalizedLandmarkList?)graphOutputs["pose_landmarks"];

            if (graphOutputs.ContainsKey("pose_world_landmarks"))
                poseOutput.PoseWorldLandmarks = (LandmarkList?)graphOutputs["pose_world_landmarks"];

            if (graphOutputs.ContainsKey("segmentation_mask"))
                poseOutput.SegmentationMask = (ImageFrame?)graphOutputs["segmentation_mask"];

            return poseOutput;
        }
    }
}
