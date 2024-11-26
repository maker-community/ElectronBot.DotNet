// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using System.Collections.Generic;
using Google.Protobuf;
using Mediapipe.Net.Core;
using Mediapipe.Net.Framework.Packets;
using Mediapipe.Net.Framework.Port;
using Mediapipe.Net.Framework.Protobuf;
using Mediapipe.Net.Native;

namespace Mediapipe.Net.Framework.ValidatedGraphConfig
{
    public unsafe class ValidatedGraphConfig : MpResourceHandle
    {
        public ValidatedGraphConfig() : base()
        {
            UnsafeNativeMethods.mp_ValidatedGraphConfig__(out var ptr).Assert();
            Ptr = ptr;
        }

        protected override void DeleteMpPtr() => UnsafeNativeMethods.mp_ValidatedGraphConfig__delete(Ptr);

        public Status Initialize(CalculatorGraphConfig config)
        {
            var bytes = config.ToByteArray();
            UnsafeNativeMethods.mp_ValidatedGraphConfig__Initialize__Rcgc(MpPtr, bytes, bytes.Length, out var statusPtr).Assert();

            GC.KeepAlive(this);
            return new Status(statusPtr);
        }

        public Status Initialize(string graphType)
        {
            UnsafeNativeMethods.mp_ValidatedGraphConfig__Initialize__PKc(MpPtr, graphType, out var statusPtr).Assert();

            GC.KeepAlive(this);
            return new Status(statusPtr);
        }

        public bool Initialized() => SafeNativeMethods.mp_ValidatedGraphConfig__Initialized(MpPtr) > 0;

        public Status ValidateRequiredSidePackets(SidePackets sidePackets)
        {
            UnsafeNativeMethods.mp_ValidatedGraphConfig__ValidateRequiredSidePackets__Rsp(MpPtr, sidePackets.MpPtr, out var statusPtr).Assert();

            GC.KeepAlive(sidePackets);
            GC.KeepAlive(this);
            return new Status(statusPtr);
        }

        public CalculatorGraphConfig Config(ExtensionRegistry? extensionRegistry = null)
        {
            UnsafeNativeMethods.mp_ValidatedGraphConfig__Config(MpPtr, out var serializedProto).Assert();
            GC.KeepAlive(this);

            var parser = extensionRegistry == null ? CalculatorGraphConfig.Parser : CalculatorGraphConfig.Parser.WithExtensionRegistry(extensionRegistry);
            var config = serializedProto.Deserialize(parser);
            serializedProto.Dispose();

            return config;
        }

        public List<EdgeInfo> InputStreamInfos()
        {
            UnsafeNativeMethods.mp_ValidatedGraphConfig__InputStreamInfos(MpPtr, out var edgeInfoVector).Assert();
            GC.KeepAlive(this);

            var edgeInfos = edgeInfoVector.Copy();
            edgeInfoVector.Dispose();
            return edgeInfos;
        }

        public List<EdgeInfo> OutputStreamInfos()
        {
            UnsafeNativeMethods.mp_ValidatedGraphConfig__OutputStreamInfos(MpPtr, out var edgeInfoVector).Assert();
            GC.KeepAlive(this);

            var edgeInfos = edgeInfoVector.Copy();
            edgeInfoVector.Dispose();
            return edgeInfos;
        }

        public List<EdgeInfo> InputSidePacketInfos()
        {
            UnsafeNativeMethods.mp_ValidatedGraphConfig__InputSidePacketInfos(MpPtr, out var edgeInfoVector).Assert();
            GC.KeepAlive(this);

            var edgeInfos = edgeInfoVector.Copy();
            edgeInfoVector.Dispose();
            return edgeInfos;
        }

        public List<EdgeInfo> OutputSidePacketInfos()
        {
            UnsafeNativeMethods.mp_ValidatedGraphConfig__OutputSidePacketInfos(MpPtr, out var edgeInfoVector).Assert();
            GC.KeepAlive(this);

            var edgeInfos = edgeInfoVector.Copy();
            edgeInfoVector.Dispose();
            return edgeInfos;
        }

        public int OutputStreamIndex(string name)
            => SafeNativeMethods.mp_ValidatedGraphConfig__OutputStreamIndex__PKc(MpPtr, name);

        public int OutputSidePacketIndex(string name)
            => SafeNativeMethods.mp_ValidatedGraphConfig__OutputSidePacketIndex__PKc(MpPtr, name);

        public int OutputStreamToNode(string name)
            => SafeNativeMethods.mp_ValidatedGraphConfig__OutputStreamToNode__PKc(MpPtr, name);

        public StatusOrString RegisteredSidePacketTypeName(string name)
        {
            UnsafeNativeMethods.mp_ValidatedGraphConfig__RegisteredSidePacketTypeName(MpPtr, name, out var statusOrStringPtr).Assert();

            GC.KeepAlive(this);
            return new StatusOrString(statusOrStringPtr);
        }

        public StatusOrString RegisteredStreamTypeName(string name)
        {
            UnsafeNativeMethods.mp_ValidatedGraphConfig__RegisteredStreamTypeName(MpPtr, name, out var statusOrStringPtr).Assert();

            GC.KeepAlive(this);
            return new StatusOrString(statusOrStringPtr);
        }

        public string? Package() => MarshalStringFromNative(UnsafeNativeMethods.mp_ValidatedGraphConfig__Package);

        public static bool IsReservedExecutorName(string name)
            => SafeNativeMethods.mp_ValidatedGraphConfig_IsReservedExecutorName(name) > 0;

        public bool IsExternalSidePacket(string name)
            => SafeNativeMethods.mp_ValidatedGraphConfig__IsExternalSidePacket__PKc(MpPtr, name) > 0;
    }
}
