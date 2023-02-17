// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using System.Collections.Generic;
using Mediapipe.Net.Core;
using Mediapipe.Net.Framework.Format;
using Mediapipe.Net.Framework.Protobuf;
using Mediapipe.Net.Gpu;
using Mediapipe.Net.Graphs.InstantMotionTracking;
using Mediapipe.Net.Native;
using Mediapipe.Net.Util;

namespace Mediapipe.Net.Framework.Packets
{
    public unsafe partial class Packet : MpResourceHandle
    {
        public bool GetBool()
        {
            UnsafeNativeMethods.mp_Packet__GetBool(MpPtr, out var value).Assert();

            GC.KeepAlive(this);
            return value;
        }

        public int GetInt()
        {
            UnsafeNativeMethods.mp_Packet__GetInt(MpPtr, out var value).Assert();

            GC.KeepAlive(this);
            return value;
        }

        public float GetFloat()
        {
            UnsafeNativeMethods.mp_Packet__GetFloat(MpPtr, out var value).Assert();

            GC.KeepAlive(this);
            return value;
        }

        public float[] GetFloatArray()
        {
            if (FloatArrayLength < 0)
                throw new InvalidOperationException("The array's length is unknown, set Length first");

            return UnsafeUtil.SafeArrayCopy(GetFloatArrayPtr(), FloatArrayLength);
        }

        public string? GetString() => MarshalStringFromNative(UnsafeNativeMethods.mp_Packet__GetString);

        public byte[] GetStringAsByteArray()
        {
            UnsafeNativeMethods.mp_Packet__GetByteString(MpPtr, out var strPtr, out var size).Assert();
            GC.KeepAlive(this);

            byte[] bytes = UnsafeUtil.SafeArrayCopy((byte*)strPtr, size);
            UnsafeNativeMethods.delete_array__PKc(strPtr);

            return bytes;
        }

        public ImageFrame GetImageFrame()
        {
            UnsafeNativeMethods.mp_Packet__GetImageFrame(MpPtr, out var imageFramePtr).Assert();

            GC.KeepAlive(this);
            return new ImageFrame(imageFramePtr, false);
        }

        public List<Anchor3d> GetAnchor3dVector()
        {
            UnsafeNativeMethods.mp_Packet__GetAnchor3dVector(MpPtr, out var anchorVector).Assert();
            GC.KeepAlive(this);

            List<Anchor3d> anchors = anchorVector.ToList();
            anchorVector.Dispose();

            return anchors;
        }

        public GpuBuffer GetGpuBuffer()
        {
            UnsafeNativeMethods.mp_Packet__GetGpuBuffer(MpPtr, out var gpuBufferPtr).Assert();

            GC.KeepAlive(this);
            return new GpuBuffer(gpuBufferPtr, false);
        }

        #region protobuf
        public ClassificationList GetClassificationList()
        {
            UnsafeNativeMethods.mp_Packet__GetClassificationList(MpPtr, out var serializedProto).Assert();
            GC.KeepAlive(this);

            var classificationList = serializedProto.Deserialize(ClassificationList.Parser);
            serializedProto.Dispose();

            return classificationList;
        }

        public List<ClassificationList> GetClassificationListVector()
        {
            UnsafeNativeMethods.mp_Packet__GetClassificationListVector(MpPtr, out var serializedProtoVector).Assert();
            GC.KeepAlive(this);

            var classificationLists = serializedProtoVector.Deserialize(ClassificationList.Parser);
            serializedProtoVector.Dispose();

            return classificationLists;
        }

        public Detection GetDetection()
        {
            UnsafeNativeMethods.mp_Packet__GetDetection(MpPtr, out var serializedProto).Assert();
            GC.KeepAlive(this);

            var detection = serializedProto.Deserialize(Detection.Parser);
            serializedProto.Dispose();

            return detection;
        }

        public List<Detection> GetDetectionVector()
        {
            UnsafeNativeMethods.mp_Packet__GetDetectionVector(MpPtr, out var serializedProtoVector).Assert();
            GC.KeepAlive(this);

            var detections = serializedProtoVector.Deserialize(Detection.Parser);
            serializedProtoVector.Dispose();

            return detections;
        }

        public FaceGeometry.FaceGeometry GetFaceGeometry()
        {
            UnsafeNativeMethods.mp_Packet__GetFaceGeometry(MpPtr, out var serializedProto).Assert();
            GC.KeepAlive(this);

            var geometry = serializedProto.Deserialize(FaceGeometry.FaceGeometry.Parser);
            serializedProto.Dispose();

            return geometry;
        }

        public List<FaceGeometry.FaceGeometry> GetFaceGeometryVector()
        {
            UnsafeNativeMethods.mp_Packet__GetFaceGeometryVector(MpPtr, out var serializedProtoVector).Assert();
            GC.KeepAlive(this);

            var geometries = serializedProtoVector.Deserialize(FaceGeometry.FaceGeometry.Parser);
            serializedProtoVector.Dispose();

            return geometries;
        }

        public FrameAnnotation GetFrameAnnotation()
        {
            UnsafeNativeMethods.mp_Packet__GetFrameAnnotation(MpPtr, out var serializedProto).Assert();
            GC.KeepAlive(this);

            var frameAnnotation = serializedProto.Deserialize(FrameAnnotation.Parser);
            serializedProto.Dispose();

            return frameAnnotation;
        }

        public LandmarkList GetLandmarkList()
        {
            UnsafeNativeMethods.mp_Packet__GetLandmarkList(MpPtr, out var serializedProto).Assert();
            GC.KeepAlive(this);

            var landmarkList = serializedProto.Deserialize(LandmarkList.Parser);
            serializedProto.Dispose();

            return landmarkList;
        }

        public List<LandmarkList> GetLandmarkListVector()
        {
            UnsafeNativeMethods.mp_Packet__GetLandmarkListVector(MpPtr, out var serializedProtoVector).Assert();
            GC.KeepAlive(this);

            var landmarkLists = serializedProtoVector.Deserialize(LandmarkList.Parser);
            serializedProtoVector.Dispose();

            return landmarkLists;
        }

        public NormalizedLandmarkList GetNormalizedLandmarkList()
        {
            UnsafeNativeMethods.mp_Packet__GetNormalizedLandmarkList(MpPtr, out var serializedProto).Assert();
            GC.KeepAlive(this);

            var normalizedLandmarkList = serializedProto.Deserialize(NormalizedLandmarkList.Parser);
            serializedProto.Dispose();

            return normalizedLandmarkList;
        }

        public List<NormalizedLandmarkList> GetNormalizedLandmarkListVector()
        {
            UnsafeNativeMethods.mp_Packet__GetNormalizedLandmarkListVector(MpPtr, out var serializedProtoVector).Assert();
            GC.KeepAlive(this);

            var normalizedLandmarkLists = serializedProtoVector.Deserialize(NormalizedLandmarkList.Parser);
            serializedProtoVector.Dispose();

            return normalizedLandmarkLists;
        }

        public Rect GetRect()
        {
            UnsafeNativeMethods.mp_Packet__GetRect(MpPtr, out var serializedProto).Assert();
            GC.KeepAlive(this);

            var rect = serializedProto.Deserialize(Rect.Parser);
            serializedProto.Dispose();

            return rect;
        }

        public List<Rect> GetRectVector()
        {
            UnsafeNativeMethods.mp_Packet__GetRectVector(MpPtr, out var serializedProtoVector).Assert();
            GC.KeepAlive(this);

            var rects = serializedProtoVector.Deserialize(Rect.Parser);
            serializedProtoVector.Dispose();

            return rects;
        }

        public NormalizedRect GetNormalizedRect()
        {
            UnsafeNativeMethods.mp_Packet__GetNormalizedRect(MpPtr, out var serializedProto).Assert();
            GC.KeepAlive(this);

            var normalizedRect = serializedProto.Deserialize(NormalizedRect.Parser);
            serializedProto.Dispose();

            return normalizedRect;
        }

        public List<NormalizedRect> GetNormalizedRectVector()
        {
            UnsafeNativeMethods.mp_Packet__GetNormalizedRectVector(MpPtr, out var serializedProtoVector).Assert();
            GC.KeepAlive(this);

            var normalizedRects = serializedProtoVector.Deserialize(NormalizedRect.Parser);
            serializedProtoVector.Dispose();

            return normalizedRects;
        }

        public TimedModelMatrixProtoList GetTimedModelMatrixProtoList()
        {
            UnsafeNativeMethods.mp_Packet__GetTimedModelMatrixProtoList(MpPtr, out var serializedProto).Assert();
            GC.KeepAlive(this);

            var matrixProtoList = serializedProto.Deserialize(TimedModelMatrixProtoList.Parser);
            serializedProto.Dispose();

            return matrixProtoList;
        }
        #endregion
    }
}
