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
    public class FaceMeshCpuSolution : CpuSolution, IComputingSolution<List<NormalizedLandmarkList>?>
    {
        private static readonly string graphPath = "mediapipe/modules/face_landmark/face_landmark_front_cpu.pbtxt";
        private static readonly string output = "multi_face_landmarks";
        public static readonly PacketType PacketType = PacketType.NormalizedLandmarkListVector;

        private static readonly IDictionary<string, PacketType> outputs = new Dictionary<string, PacketType>()
        {
            { output, PacketType }
        };

        private static SidePackets toSidePackets(bool staticImageMode, int numFaces, bool refineLandmarks)
        {
            SidePackets sidePackets = new();
            sidePackets.Emplace("use_prev_landmarks", PacketFactory.BoolPacket(!staticImageMode));
            sidePackets.Emplace("num_faces", PacketFactory.IntPacket(numFaces));
            sidePackets.Emplace("with_attention", PacketFactory.BoolPacket(refineLandmarks));
            return sidePackets;
        }

        public FaceMeshCpuSolution(
            bool staticImageMode = false,
            int maxNumFaces = 1,
            bool refineLandmarks = false
        ) : base(graphPath, "image", outputs, toSidePackets(staticImageMode, maxNumFaces, refineLandmarks))
        {
        }

        public List<NormalizedLandmarkList>? Compute(ImageFrame frame)
        {
            IDictionary<string, object?> graphOutputs = ProcessFrame(frame);

            if (graphOutputs.ContainsKey(output))
                return (List<NormalizedLandmarkList>?)graphOutputs[output];
            else
                return null;
        }
    }
}
