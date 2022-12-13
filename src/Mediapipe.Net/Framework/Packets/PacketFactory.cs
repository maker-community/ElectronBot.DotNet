// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using Mediapipe.Net.Framework.Format;
using Mediapipe.Net.Gpu;
using Mediapipe.Net.Graphs.InstantMotionTracking;
using Mediapipe.Net.Native;

namespace Mediapipe.Net.Framework.Packets
{
    public static unsafe class PacketFactory
    {
        public static Packet BoolPacket(bool value)
        {
            UnsafeNativeMethods.mp__MakeBoolPacket__b(value, out var ptr).Assert();
            return new Packet(ptr)
            {
                PacketType = PacketType.Bool
            };
        }

        public static Packet IntPacket(int value)
        {
            UnsafeNativeMethods.mp__MakeIntPacket__i(value, out var ptr).Assert();
            return new Packet(ptr)
            {
                PacketType = PacketType.Int
            };
        }

        public static Packet FloatPacket(float value)
        {
            UnsafeNativeMethods.mp__MakeFloatPacket__f(value, out var ptr).Assert();
            return new Packet(ptr)
            {
                PacketType = PacketType.Float
            };
        }

        public static Packet FloatArrayPacket(float[] value)
        {
            UnsafeNativeMethods.mp__MakeFloatArrayPacket__Pf_i(value, value.Length, out var ptr).Assert();
            return new Packet(ptr)
            {
                PacketType = PacketType.FloatArray,
                FloatArrayLength = value.Length
            };
        }

        public static Packet FloatArrayPacket(float[] value, Timestamp timestamp)
        {
            UnsafeNativeMethods.mp__MakeFloatArrayPacket_At__Pf_i_Rt(value, value.Length, timestamp.MpPtr, out var ptr).Assert();
            GC.KeepAlive(timestamp);
            return new Packet(ptr)
            {
                PacketType = PacketType.FloatArray,
                FloatArrayLength = value.Length
            };
        }

        public static Packet StringPacket(string value)
        {
            UnsafeNativeMethods.mp__MakeStringPacket__PKc(value, out var ptr).Assert();
            return new Packet(ptr)
            {
                PacketType = PacketType.String
            };
        }

        public static Packet StringPacket(byte[] bytes)
        {
            UnsafeNativeMethods.mp__MakeStringPacket__PKc_i(bytes, bytes.Length, out var ptr).Assert();
            return new Packet(ptr)
            {
                PacketType = PacketType.String
            };
        }

        public static Packet ImageFramePacket(ImageFrame imageFrame)
        {
            UnsafeNativeMethods.mp__MakeImageFramePacket__Pif(imageFrame.MpPtr, out var ptr).Assert();
            imageFrame.Dispose(); // respect move semantics
            return new Packet(ptr)
            {
                PacketType = PacketType.ImageFrame
            };
        }

        /// <summary>
        /// Use this instead of <c>ImageFramePacket(imageFrame).At(timestamp)</c>.
        /// Somehow it fails unit tests, so better be safe.
        /// </summary>
        public static Packet ImageFramePacket(ImageFrame imageFrame, Timestamp timestamp)
        {
            UnsafeNativeMethods.mp__MakeImageFramePacket_At__Pif_Rt(imageFrame.MpPtr, timestamp.MpPtr, out var ptr).Assert();
            GC.KeepAlive(timestamp);
            imageFrame.Dispose(); // respect move semantics
            return new Packet(ptr)
            {
                PacketType = PacketType.ImageFrame
            };
        }

        public static Packet Anchor3dVectorPacket(Anchor3d[] value)
        {
            UnsafeNativeMethods.mp__MakeAnchor3dVectorPacket__PA_i(value, value.Length, out var ptr).Assert();
            return new Packet(ptr)
            {
                PacketType = PacketType.Anchor3dVector
            };
        }

        public static Packet GpuBufferPacket(GpuBuffer gpuBuffer)
        {
            UnsafeNativeMethods.mp__MakeGpuBufferPacket__Rgb(gpuBuffer.MpPtr, out var ptr).Assert();
            gpuBuffer.Dispose(); // respect move semantics
            return new Packet(ptr)
            {
                PacketType = PacketType.GpuBuffer
            };
        }

        public static Packet GpuBufferPacket(GpuBuffer gpuBuffer, Timestamp timestamp)
        {
            UnsafeNativeMethods.mp__MakeGpuBufferPacket_At__Rgb_Rts(gpuBuffer.MpPtr, timestamp.MpPtr, out var ptr).Assert();
            GC.KeepAlive(timestamp);
            gpuBuffer.Dispose(); // respect move semantics
            return new Packet(ptr)
            {
                PacketType = PacketType.GpuBuffer
            };
        }
    }
}
