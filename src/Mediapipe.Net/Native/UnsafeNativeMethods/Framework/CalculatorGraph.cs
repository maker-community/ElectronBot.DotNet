// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Runtime.InteropServices;
using Mediapipe.Net.External;
using Mediapipe.Net.Framework;

namespace Mediapipe.Net.Native
{
    internal unsafe partial class UnsafeNativeMethods : NativeMethods
    {
#pragma warning disable CA2101
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_CalculatorGraph__(out void* graph);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_CalculatorGraph__PKc_i(byte[] serializedConfig, int size, out void* graph);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void mp_CalculatorGraph__delete(void* graph);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_CalculatorGraph__Initialize__PKc_i(void* graph, byte[] serializedConfig, int size, out void* status);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_CalculatorGraph__Initialize__PKc_i_Rsp(
            void* graph, byte[] serializedConfig, int size, void* sidePackets, out void* status);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_CalculatorGraph__Config(void* graph, out SerializedProto serializedProto);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern MpReturnCode mp_CalculatorGraph__ObserveOutputStream__PKc_PF_b(void* graph, string streamName,
            int streamId, CalculatorGraph.NativePacketCallback packetCallback,
            bool observeTimestampBounds, out void* status);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern MpReturnCode mp_CalculatorGraph__AddOutputStreamPoller__PKc_b(void* graph, string streamName,
            bool observeTimestampBounds, out void* statusOrPoller);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_CalculatorGraph__Run__Rsp(void* graph, void* sidePackets, out void* status);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_CalculatorGraph__StartRun__Rsp(void* graph, void* sidePackets, out void* status);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_CalculatorGraph__WaitUntilIdle(void* graph, out void* status);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_CalculatorGraph__WaitUntilDone(void* graph, out void* status);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern MpReturnCode mp_CalculatorGraph__AddPacketToInputStream__PKc_Ppacket(
            void* graph, string streamName, void* packet, out void* status);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern MpReturnCode mp_CalculatorGraph__SetInputStreamMaxQueueSize__PKc_i(
            void* graph, string streamName, int maxQueueSize, out void* status);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern MpReturnCode mp_CalculatorGraph__CloseInputStream__PKc(void* graph, string streamName, out void* status);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_CalculatorGraph__CloseAllPacketSources(void* graph, out void* status);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_CalculatorGraph__Cancel(void* graph);

        #region GPU
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_CalculatorGraph__GetGpuResources(void* graph, out void* gpuResources);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_CalculatorGraph__SetGpuResources__SPgpu(void* graph, void* gpuResources, out void* status);
        #endregion
#pragma warning restore CA2101
    }
}
