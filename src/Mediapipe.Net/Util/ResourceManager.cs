// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System;
using Mediapipe.Net.External;
using Mediapipe.Net.Native;

namespace Mediapipe.Net.Util
{
    /// <summary>
    /// Class to manage MediaPipe resources, such as `.tflite` and `.pbtxt` files that it requests.
    /// </summary>
    /// <remarks>
    /// There must not be more than one instance at the same time.
    /// </remarks>
    public unsafe abstract class ResourceManager
    {
        public delegate string PathResolver(string path);

        /// <summary>
        /// Resolves a path to a resource name.
        /// If the resource name returned is different from the path, the <see cref="ResourceProvider" /> delegate will receive the resource name instead of the file path.
        /// </summary>
        public abstract PathResolver ResolvePath { get; }

        /// <summary>
        /// Reads a resource that MediaPipe requests.
        /// </summary>
        /// <param name="path">File path or name of the resource.</param>
        /// <returns>Content of the MediaPipe resource as a byte array.</returns>
        public delegate byte[] ResourceProvider(string path);
        public abstract ResourceProvider ProvideResource { get; }

        private static readonly object initLock = new object();
        private static bool isInitialized = false;

        public ResourceManager()
        {
            lock (initLock)
            {
                if (isInitialized)
                    throw new InvalidOperationException("ResourceManager can be initialized only once");

                SafeNativeMethods.mp__SetCustomGlobalPathResolver__P(ResolvePath);
                SafeNativeMethods.mp__SetCustomGlobalResourceProvider__P(provideResource);
                isInitialized = true;
            }
        }

        private bool provideResource(string path, void* output)
        {
            try
            {
                byte[] bytes = ProvideResource(path);

                StdString strOutput = new StdString(output, isOwner: false);
                StdString strSpan = new StdString(bytes);
                strOutput.Swap(strSpan);

                return true;
            }
            catch (Exception ex)
            {
                Glog.Log(Glog.Severity.Error, $"Error while trying to provide resource '{path}': {ex}");
                return false;
            }
        }
    }
}
