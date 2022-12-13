// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Runtime.InteropServices;

namespace Mediapipe.Net.Native
{
    internal unsafe partial class UnsafeNativeMethods : NativeMethods
    {
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void delete_array__PKc(sbyte* str);

        #region String
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void std_string__delete(void* str);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode std_string__PKc_i(byte[] bytes, int size, out void* str);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void std_string__swap__Rstr(void* src, void* dst);
        #endregion

        #region StatusOrString
        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern void mp_StatusOrString__delete(void* statusOrString);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_StatusOrString__status(void* statusOrString, out void* status);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_StatusOrString__value(void* statusOrString, out sbyte* value);

        [DllImport(MEDIAPIPE_LIBRARY, ExactSpelling = true)]
        public static extern MpReturnCode mp_StatusOrString__bytearray(void* statusOrString, out sbyte* value, out int size);
        #endregion
    }
}
