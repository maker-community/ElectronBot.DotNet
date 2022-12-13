// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using Google.Protobuf;
using Mediapipe.Net.Native;

namespace Mediapipe.Net.External
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe readonly struct SerializedProtoVector
    {
        public readonly SerializedProto* Data;
        public readonly int Size;

        // The array element freeing loop has been moved to MediaPipe.NET.Runtime.
        public void Dispose() => UnsafeNativeMethods.mp_api_SerializedProtoArray__delete(Data, Size);

        public List<T> Deserialize<T>(MessageParser<T> parser) where T : IMessage<T>
        {
            var protos = new List<T>(Size);
            for (int i = 0; i < Size; i++)
                protos.Add(Data[i].Deserialize(parser));

            return protos;
        }
    }
}
