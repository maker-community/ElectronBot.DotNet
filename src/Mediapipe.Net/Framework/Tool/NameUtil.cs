#pragma warning disable IDE0073
// Copyright 2019 The MediaPipe Authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using System.Linq;
using Mediapipe.Net.Framework.Protobuf;

namespace Mediapipe.Net.Framework.Tool
{
    /// <summary>
    /// translated version of mediapipe/framework/tool/name_util.cc
    /// <summary/>
    public static partial class Tool
    {
        public static string GetUnusedNodeName(CalculatorGraphConfig config, string nodeNameBase)
        {
            var nodeNames = new HashSet<string>(config.Node.Select(node => node.Name).Where(name => name.Length > 0));
            var candidate = nodeNameBase;

            for (int i = 2; nodeNames.Contains(candidate); i++)
                candidate = $"{nodeNameBase}_{i:D2}";

            return candidate;
        }

        public static string GetUnusedSidePacketName(CalculatorGraphConfig config, string inputSidePacketNameBase)
        {
            var inputSidePackets = new HashSet<string>(
              config.Node.SelectMany(node => node.InputSidePacket)
                .Select(sidePacket =>
                {
                    ParseTagIndexName(sidePacket, out var tag, out var index, out var name);
                    return name;
                }));

            var candidate = inputSidePacketNameBase;

            for (int i = 2; inputSidePackets.Contains(candidate); i++)
                candidate = $"{inputSidePacketNameBase}_{i:D2}";

            return candidate;
        }

        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="nodeId" /> is invalid
        /// </exception>
        public static string CanonicalNodeName(CalculatorGraphConfig graphConfig, int nodeId)
        {
            CalculatorGraphConfig.Types.Node nodeConfig = graphConfig.Node[nodeId];
            string nodeName = nodeConfig.Name.Length == 0 ? nodeConfig.Calculator : nodeConfig.Name;

            var nodesWithSameName = graphConfig.Node
              .Select((node, i) => (node.Name.Length == 0 ? node.Calculator : node.Name, i))
              .Where(pair => pair.Item1 == nodeName);

            if (nodesWithSameName.Count() <= 1)
                return nodeName;

            var seq = nodesWithSameName.Count(pair => pair.i <= nodeId);
            return $"{nodeName}_{seq}";
        }

        /// <exception cref="ArgumentException">
        /// Thrown when the format of <paramref cref="stream" /> is invalid
        /// </exception>
        public static string ParseNameFromStream(string stream)
        {
            ParseTagIndexName(stream, out var _, out var _, out var name);
            return name;
        }

        /// <exception cref="ArgumentException">
        /// Thrown when the format of <paramref cref="tagIndex" /> is invalid
        /// </exception>
        public static (string, int) ParseTagIndex(string tagIndex)
        {
            ParseTagIndex(tagIndex, out var tag, out var index);
            return (tag, index);
        }

        /// <exception cref="ArgumentException">
        /// Thrown when the format of <paramref cref="stream" /> is invalid
        /// </exception>
        public static (string, int) ParseTagIndexFromStream(string stream)
        {
            ParseTagIndexName(stream, out var tag, out var index, out var _);
            return (tag, index);
        }

        public static string CatTag(string tag, int index)
        {
            string colonIndex = index <= 0 || tag.Length == 0 ? "" : $":{index}";
            return $"{tag}{colonIndex}";
        }

        public static string CatStream((string, int) tagIndex, string name)
        {
            string tag = CatTag(tagIndex.Item1, tagIndex.Item2);
            return tag.Length == 0 ? name : $"{tag}:{name}";
        }
    }
}
#pragma warning restore IDE0073
