// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Collections.Generic;
using System.Runtime.Versioning;
using Mediapipe.Net.Core;
using Mediapipe.Net.Framework;
using Mediapipe.Net.Framework.Format;
using Mediapipe.Net.Framework.Packets;
using Mediapipe.Net.Gpu;

namespace Mediapipe.Net.Solutions
{
    /// <summary>
    /// Computer Vision solution running on the CPU.
    /// </summary>
    /// <typeparam name="TPacket">The type of packet the calculator returns the secondary output in.</typeparam>
    /// <typeparam name="T">The type of secondary output.</typeparam>
    [SupportedOSPlatform("Linux"), SupportedOSPlatform("Android")]
    public abstract class GpuSolution : Solution
    {
        public readonly string GpuBufferInput;

        private readonly GpuResources gpuResources;
        private readonly GlCalculatorHelper gpuHelper;

        protected GpuSolution(string graphPath, string gpuBufferInput, IDictionary<string, PacketType> outputs, SidePackets? sidePackets) : base(graphPath, outputs, sidePackets)
        {
            GpuBufferInput = gpuBufferInput;

            gpuResources = GpuResources.Create().Value();
            Graph.SetGpuResources(gpuResources);
            gpuHelper = new GlCalculatorHelper();
            gpuHelper.InitializeForTest(Graph.GetGpuResources());
        }

        protected override IDictionary<string, object?> ProcessFrame(ImageFrame frame)
        {
            Dictionary<string, Packet> inputs = new();

            gpuHelper.RunInGlContext(() =>
            {
                GlTexture texture = gpuHelper.CreateSourceTexture(frame);
                GpuBuffer gpuBuffer = texture.GetGpuBufferFrame();
                Gl.Flush();
                texture.Release();

                Timestamp timestamp = new Timestamp(SimulatedTimestamp);
                Packet packet = PacketFactory.GpuBufferPacket(gpuBuffer, timestamp);
                inputs.Add(GpuBufferInput, packet);
            }).AssertOk();

            return Process(inputs);
        }

        protected ImageFrame GetImageFrameFromGpuBuffer(Packet gpuBufferPacket)
        {
            ImageFrame? outFrame = null;

            gpuHelper.RunInGlContext(() =>
            {
                GpuBuffer outBuffer = gpuBufferPacket.GetGpuBuffer();
                GlTexture texture = gpuHelper.CreateSourceTexture(outBuffer);
                outFrame = new ImageFrame(outBuffer.Format.ImageFormatFor(), outBuffer.Width, outBuffer.Height, ImageFrame.GlDefaultAlignmentBoundary);
                gpuHelper.BindFramebuffer(texture);
                GlTextureInfo info = outBuffer.Format.GlTextureInfoFor(0);
                unsafe
                {
                    Gl.ReadPixels(0, 0, texture.Width, texture.Height, info.GlFormat, info.GlType, outFrame.MutablePixelData);
                }
                Gl.Flush();
                texture.Release();
            }).AssertOk();

            if (outFrame == null)
                throw new MediapipeNetException("!! FATAL - Frame is null on current GL context run!");

            return outFrame;
        }
    }
}
