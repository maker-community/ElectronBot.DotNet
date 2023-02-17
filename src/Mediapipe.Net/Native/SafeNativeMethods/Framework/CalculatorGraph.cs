// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace Mediapipe.Net.Native
{
    internal unsafe partial class SafeNativeMethods : NativeMethods
    {
#pragma warning disable CA2101
        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern byte mp_CalculatorGraph__HasError(void* graph);

        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern byte mp_CalculatorGraph__HasInputStream__PKc(void* graph, string name);

        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern byte mp_CalculatorGraph__GraphInputStreamsClosed(void* graph);

        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern byte mp_CalculatorGraph__IsNodeThrottled__i(void* graph, int nodeId);

        [Pure, DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern byte mp_CalculatorGraph__UnthrottleSources(void* graph);
#pragma warning restore CA2101
    }
}
