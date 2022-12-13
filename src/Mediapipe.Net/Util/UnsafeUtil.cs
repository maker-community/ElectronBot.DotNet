// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

namespace Mediapipe.Net.Util
{
    internal unsafe static class UnsafeUtil
    {
        /// <summary>
        /// Copies all objects from an unsafe array to a safe array.
        /// </summary>
        /// <param name="ptr">The pointer array of objects.</param>
        /// <param name="length">The number of elements inside of the unsafe array.</param>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <remarks>
        /// It doesn't do any kind of deep copy of the elements.
        /// So if the object of type T contains references to other objects,
        /// the references will be copied and not the objects themselves.
        /// </remarks>
        /// <returns>A safe array containing a copy of the elements from the unsafe array.</returns>
        public static T[] SafeArrayCopy<T>(T* ptr, int length)
            where T : unmanaged
        {
            T[] array = new T[length];
            for (int i = 0; i < array.Length; i++)
                array[i] = ptr[i];
            return array;
        }
    }
}
