// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Mediapipe.Net.Core;
using Mediapipe.Net.Framework;
using Mediapipe.Net.Framework.Format;
using Mediapipe.Net.Framework.Packets;

namespace Mediapipe.Net.Solutions
{
    /// <summary>
    /// The base for any calculator.
    /// </summary>
    /// <typeparam name="TPacket">The type of packet the calculator returns the secondary output in.</typeparam>
    /// <typeparam name="T">The type of secondary output.</typeparam>
    public abstract class Solution : Disposable
    {
        protected readonly string GraphPath;
        protected readonly CalculatorGraph Graph;
        protected readonly SidePackets? SidePackets;

        protected readonly IDictionary<string, object?> GraphOutputs;
        private readonly IDictionary<string, GCHandle> observeStreamHandles;

        protected long SimulatedTimestamp = 0;

        protected Solution(
            string graphPath,
            IDictionary<string, PacketType> outputs,
            SidePackets? sidePackets)
        {
            GraphPath = graphPath;
            Graph = new CalculatorGraph(File.ReadAllText(GraphPath));
            SidePackets = sidePackets;

            GraphOutputs = new Dictionary<string, object?>();

            observeStreamHandles = new Dictionary<string, GCHandle>();
            foreach ((string output, PacketType packetType) in outputs)
            {
                Graph.ObserveOutputStream(output, (packet) =>
                {
                    packet.PacketType = packetType;
                    lock (GraphOutputs)
                        GraphOutputs.Add(output, packet.Get());
                }, out GCHandle handle).AssertOk();
                observeStreamHandles.Add(output, handle);
            }

            Graph.StartRun(SidePackets).AssertOk();
        }

        /// <summary>
        /// Feeds each input packet to the calculator graph and returns all outputs.
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        protected IDictionary<string, object?> Process(IDictionary<string, Packet> inputs)
        {
            // Set the timestamp increment to 16666 us to simulate 60 fps video input (?)
            // That's what the Python API does so ¯\_(ツ)_/¯
            // Might have to find something better?
            SimulatedTimestamp += 10000;

            // Dispose of the previous packets before processing
            foreach (object? obj in GraphOutputs.Values)
            {
                if (obj is IDisposable disposable)
                    disposable.Dispose();
            }
            GraphOutputs.Clear();

            foreach (KeyValuePair<string, Packet> input in inputs)
            {
                if (input.Value != null)
                    Graph.AddPacketToInputStream(input.Key, input.Value);
            }

            Graph.WaitUntilIdle();
            return GraphOutputs;
        }

        protected abstract IDictionary<string, object?> ProcessFrame(ImageFrame frame);

        /// <summary>
        /// Closes all the input sources and the graph.
        /// </summary>
        public void Close() => Graph.CloseAllPacketSources().AssertOk();

        public void Reset()
        {
            Graph.CloseAllPacketSources().AssertOk();
            Graph.StartRun(SidePackets).AssertOk();
        }

        protected override void DisposeManaged()
        {
            Graph.WaitUntilDone();
            Graph.Dispose();

            SidePackets?.Dispose();

            foreach (var handle in observeStreamHandles.Values)
                handle.Free();
        }
    }
}
