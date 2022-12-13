// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using Mediapipe.Net.Core;

namespace Mediapipe.Net.Native
{
    public static class MpReturnCodeExtension
    {
        public static void Assert(this MpReturnCode code)
        {
            switch (code)
            {
                case MpReturnCode.Success:
                    return;
                case MpReturnCode.Aborted:
                    throw new MediapipeException("MediaPipe Aborted, refer glog files for more details");
                case MpReturnCode.StandardError:
                    throw new MediapipeNetException("Exception is thrown in Unmanaged Code");
                case MpReturnCode.UnknownError:
                    throw new MediapipeNetException("Unknown exception is thrown in Unmanaged Code");
                case MpReturnCode.Unset:
                    // Bug
                    throw new MediapipeNetException("Failed to call a native function, but the reason is unknown");
                default:
                    throw new MediapipeNetException("Failed to call a native function, but the reason is undefined");
            }
        }
    }
}
