// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using Mediapipe.Net.Core;
using Mediapipe.Net.Framework.Format;
using Mediapipe.Net.Framework.Port;
using Mediapipe.Net.Gpu;
using Mediapipe.Net.Native;

namespace Mediapipe.Net.Framework.Packets
{
    public unsafe partial class Packet : MpResourceHandle
    {
        public PacketType PacketType { get; internal set; }

        internal Packet() : base()
        {
            UnsafeNativeMethods.mp_Packet__(out var ptr).Assert();
            Ptr = ptr;
        }

        internal Packet(void* ptr, bool isOwner = true) : base(ptr, isOwner) { }

        // Temp backwards compatibility until we find something better than the Activator. ¬¬
        internal Packet(IntPtr ptr, bool isOwner = true) : base((void*)ptr, isOwner) { }


        /// <remarks>To avoid copying the value, instantiate the packet with timestamp</remarks>
        /// <returns>New packet with the given timestamp and the copied value</returns>
        public Packet At(Timestamp timestamp)
        {
            UnsafeNativeMethods.mp_Packet__At__Rt(MpPtr, timestamp.MpPtr, out var packetPtr).Assert();
            GC.KeepAlive(timestamp);

            return new Packet((IntPtr)packetPtr, true)
            {
                PacketType = PacketType
            };
        }

        public bool IsEmpty() => SafeNativeMethods.mp_Packet__IsEmpty(MpPtr) > 0;

        public Status ValidateAsProtoMessageLite()
        {
            UnsafeNativeMethods.mp_Packet__ValidateAsProtoMessageLite(MpPtr, out var statusPtr).Assert();

            GC.KeepAlive(this);
            return new Status(statusPtr);
        }

        public Timestamp Timestamp()
        {
            UnsafeNativeMethods.mp_Packet__Timestamp(MpPtr, out var timestampPtr).Assert();

            GC.KeepAlive(this);
            return new Timestamp(timestampPtr);
        }

        public string? DebugString() => MarshalStringFromNative(UnsafeNativeMethods.mp_Packet__DebugString);

        public string RegisteredTypeName()
        {
            string? typeName = MarshalStringFromNative(UnsafeNativeMethods.mp_Packet__RegisteredTypeName);

            return typeName ?? "";
        }

        public string? DebugTypeName()
        {
            string? typeName = MarshalStringFromNative(UnsafeNativeMethods.mp_Packet__DebugTypeName);

            return typeName ?? "";
        }

        protected override void DeleteMpPtr() => UnsafeNativeMethods.mp_Packet__delete(Ptr);

        public object? Get()
        {
            return PacketType switch
            {
                PacketType.Bool => GetBool(),
                PacketType.Int => GetInt(),
                PacketType.Float => GetFloat(),
                PacketType.FloatArray => GetFloatArray(),
                PacketType.String => GetString(),
                PacketType.StringAsByteArray => GetStringAsByteArray(),
                PacketType.ImageFrame => GetImageFrame(),
                PacketType.Anchor3dVector => GetAnchor3dVector(),
                PacketType.GpuBuffer => GetGpuBuffer(),
                PacketType.ClassificationList => GetClassificationList(),
                PacketType.ClassificationListVector => GetClassificationListVector(),
                PacketType.Detection => GetDetection(),
                PacketType.DetectionVector => GetDetectionVector(),
                PacketType.FaceGeometry => GetFaceGeometry(),
                PacketType.FaceGeometryVector => GetFaceGeometryVector(),
                PacketType.FrameAnnotation => GetFrameAnnotation(),
                PacketType.LandmarkList => GetLandmarkList(),
                PacketType.LandmarkListVector => GetLandmarkListVector(),
                PacketType.NormalizedLandmarkList => GetNormalizedLandmarkList(),
                PacketType.NormalizedLandmarkListVector => GetNormalizedLandmarkListVector(),
                PacketType.Rect => GetRect(),
                PacketType.RectVector => GetRectVector(),
                PacketType.NormalizedRect => GetNormalizedRect(),
                PacketType.NormalizedRectVector => GetNormalizedRectVector(),
                PacketType.TimedModelMatrixProtoList => GetTimedModelMatrixProtoList(),
                _ => throw new NotImplementedException(),
            };
        }

        #region FloatArray
        public int FloatArrayLength { get; init; }

        internal float* GetFloatArrayPtr()
        {
            UnsafeNativeMethods.mp_Packet__GetFloatArray(MpPtr, out float* array).Assert();
            GC.KeepAlive(this);
            return array;
        }
        #endregion

        #region Consumers
        public StatusOr<ImageFrame> ConsumeImageFrame()
        {
            UnsafeNativeMethods.mp_Packet__ConsumeImageFrame(MpPtr, out var statusOrImageFramePtr).Assert();

            GC.KeepAlive(this);
            return new StatusOrImageFrame(statusOrImageFramePtr);
        }

        public StatusOr<GpuBuffer> ConsumeGpuBuffer()
        {
            UnsafeNativeMethods.mp_Packet__ConsumeGpuBuffer(MpPtr, out var statusOrGpuBufferPtr).Assert();

            GC.KeepAlive(this);
            return new StatusOrGpuBuffer(statusOrGpuBufferPtr);
        }

        public StatusOr<string> ConsumeString()
        {
            UnsafeNativeMethods.mp_Packet__ConsumeString(MpPtr, out var statusOrStringPtr).Assert();

            GC.KeepAlive(this);
            return new StatusOrString(statusOrStringPtr);
        }
        #endregion

        #region Validators
        public Status ValidateAsBool()
        {
            UnsafeNativeMethods.mp_Packet__ValidateAsBool(MpPtr, out var statusPtr).Assert();

            GC.KeepAlive(this);
            return new Status(statusPtr);
        }

        public Status ValidateAsInt()
        {
            UnsafeNativeMethods.mp_Packet__ValidateAsInt(MpPtr, out var statusPtr).Assert();

            GC.KeepAlive(this);
            return new Status(statusPtr);
        }

        public Status ValidateAsFloat()
        {
            UnsafeNativeMethods.mp_Packet__ValidateAsFloat(MpPtr, out var statusPtr).Assert();

            GC.KeepAlive(this);
            return new Status(statusPtr);
        }

        public Status ValidateAsFloatArray()
        {
            UnsafeNativeMethods.mp_Packet__ValidateAsFloatArray(MpPtr, out var statusPtr).Assert();

            GC.KeepAlive(this);
            return new Status(statusPtr);
        }

        public Status ValidateAsString()
        {
            UnsafeNativeMethods.mp_Packet__ValidateAsString(MpPtr, out var statusPtr).Assert();

            GC.KeepAlive(this);
            return new Status(statusPtr);
        }

        public Status ValidateAsGpuBuffer()
        {
            UnsafeNativeMethods.mp_Packet__ValidateAsGpuBuffer(MpPtr, out var statusPtr).Assert();

            GC.KeepAlive(this);
            return new Status(statusPtr);
        }

        public Status ValidateAsImageFrame()
        {
            UnsafeNativeMethods.mp_Packet__ValidateAsImageFrame(MpPtr, out var statusPtr).Assert();

            GC.KeepAlive(this);
            return new Status(statusPtr);
        }
        #endregion
    }
}
