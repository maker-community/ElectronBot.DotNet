// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// PreviewCameraChanged Event Args
/// </summary>
public class PreviewCameraChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets error information about failure
    /// </summary>
    public string? CameraName { get; internal set; }
}
