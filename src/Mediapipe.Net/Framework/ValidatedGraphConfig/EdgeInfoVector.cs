// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mediapipe.Net.Native;

namespace Mediapipe.Net.Framework.ValidatedGraphConfig
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe readonly struct EdgeInfoVector
    {
        private readonly void* data;
        private readonly int size;

        public void Dispose() => UnsafeNativeMethods.mp_api_EdgeInfoArray__delete(data, size);

        public List<EdgeInfo> Copy()
        {
            var edgeInfos = new List<EdgeInfo>(size);

            unsafe
            {
                var edgeInfoPtr = (EdgeInfoTmp*)data;

                for (int i = 0; i < size; i++)
                {
                    EdgeInfoTmp edgeInfoTmp = Marshal.PtrToStructure<EdgeInfoTmp>((IntPtr)edgeInfoPtr++);
                    edgeInfos.Add(edgeInfoTmp.Copy());
                }
            }

            return edgeInfos;
        }

        [StructLayout(LayoutKind.Sequential)]
        private readonly struct EdgeInfoTmp
        {
            private readonly int upstream;
            private readonly NodeRef parentNode;
            private readonly IntPtr name;

            [MarshalAs(UnmanagedType.U1)]
            private readonly bool backEdge;

            public EdgeInfo Copy()
            {
                string? name = Marshal.PtrToStringAnsi(this.name);
                return new EdgeInfo(upstream, parentNode, name, backEdge);
            }
        }
    }
}
