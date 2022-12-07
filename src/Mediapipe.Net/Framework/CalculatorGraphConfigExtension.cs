// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using Google.Protobuf;
using Mediapipe.Net.Core;
using Mediapipe.Net.Framework.Protobuf;
using Mediapipe.Net.Native;

namespace Mediapipe.Net.Framework
{
    public static class CalculatorGraphConfigExtension
    {
        public static CalculatorGraphConfig ParseFromTextFormat(this MessageParser<CalculatorGraphConfig> _, string configText)
        {
            if (UnsafeNativeMethods.mp_api__ConvertFromCalculatorGraphConfigTextFormat(configText, out var serializedProto) > 0)
            {
                var config = serializedProto.Deserialize(CalculatorGraphConfig.Parser);
                serializedProto.Dispose();
                return config;
            }
            throw new MediapipeException("Failed to parse config text. See error logs for more details");
        }
    }
}
