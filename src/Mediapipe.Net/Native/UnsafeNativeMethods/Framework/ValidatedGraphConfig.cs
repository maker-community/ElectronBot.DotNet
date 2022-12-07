// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Runtime.InteropServices;
using Mediapipe.Net.External;
using Mediapipe.Net.Framework.ValidatedGraphConfig;

namespace Mediapipe.Net.Native
{
    internal unsafe partial class UnsafeNativeMethods
    {
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_ValidatedGraphConfig__(out void* config);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void mp_ValidatedGraphConfig__delete(void* config);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_ValidatedGraphConfig__Initialize__Rcgc(void* config, byte[] serializedConfig, int size, out void* status);

#pragma warning disable CA2101
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern MpReturnCode mp_ValidatedGraphConfig__Initialize__PKc(void* config, string graphType, out void* status);
#pragma warning restore CA2101

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_ValidatedGraphConfig__ValidateRequiredSidePackets__Rsp(void* config, void* sidePackets, out void* status);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_ValidatedGraphConfig__Config(void* config, out SerializedProto serializedProto);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_ValidatedGraphConfig__InputStreamInfos(void* config, out EdgeInfoVector edgeInfoVector);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_ValidatedGraphConfig__OutputStreamInfos(void* config, out EdgeInfoVector edgeInfoVector);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_ValidatedGraphConfig__InputSidePacketInfos(void* config, out EdgeInfoVector edgeInfoVector);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_ValidatedGraphConfig__OutputSidePacketInfos(void* config, out EdgeInfoVector edgeInfoVector);

#pragma warning disable CA2101
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern MpReturnCode mp_ValidatedGraphConfig__RegisteredSidePacketTypeName(void* config, string name, out void* statusOrString);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern MpReturnCode mp_ValidatedGraphConfig__RegisteredStreamTypeName(void* config, string name, out void* statusOrString);
#pragma warning restore CA2101

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_ValidatedGraphConfig__Package(void* config, out sbyte* str);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void mp_api_EdgeInfoArray__delete(void* data, int size);
    }
}
