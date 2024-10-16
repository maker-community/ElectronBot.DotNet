// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Vedure.Braincsse.WinUI.Helpers;
using Windows.Media.Capture.Frames;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace CommunityToolkit.WinUI.Controls;

/// Camera Control to preview video. Can subscribe to video frames, software bitmap when they arrive.
/// </summary>
[TemplatePart(Name = Preview_MediaPlayerElementControl, Type = typeof(MediaPlayerElement))]
[TemplatePart(Name = Preview_FrameSourceGroupButton, Type = typeof(Button))]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "Implemented via Stop()")]
public partial class CameraPreview : Control
{
    private CameraHelper _cameraHelper;
    private MediaPlayer _mediaPlayer;
    private MediaPlayerElement _mediaPlayerElementControl;
    private Button _frameSourceGroupButton;

    private IReadOnlyList<MediaFrameSourceGroup> _frameSourceGroups;

    private bool IsFrameSourceGroupButtonAvailable => _frameSourceGroups != null && _frameSourceGroups.Count > 1;


    public MediaPlayer MediaPlayer
    {
        get => _mediaPlayer; private set => _mediaPlayer = value;
    }


    /// <summary>
    /// Gets Camera Helper
    /// </summary>
    public CameraHelper CameraHelper
    {
        get => _cameraHelper; private set => _cameraHelper = value;
    }

    /// <summary>
    /// Initialize control with a default CameraHelper instance for video preview and frame capture.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task StartAsync()
    {
        await StartAsync(new CameraHelper());
    }

    /// <summary>
    /// Initialize control with a CameraHelper instance for video preview and frame capture.
    /// </summary>
    /// <param name="cameraHelper"><see cref="CameraHelper"/></param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task StartAsync(CameraHelper cameraHelper)
    {
        if (cameraHelper == null)
        {
            cameraHelper = new CameraHelper();
        }

        _cameraHelper = cameraHelper;
        _frameSourceGroups = await CameraHelper.GetFrameSourceGroupsAsync();

        // UI controls exist and are initialized
        if (_mediaPlayerElementControl != null)
        {
            await InitializeAsync();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CameraPreview"/> class.
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public CameraPreview()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        this.DefaultStyleKey = typeof(CameraPreview);
    }

    /// <inheritdoc/>
    protected async override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (_frameSourceGroupButton != null)
        {
            _frameSourceGroupButton.Click -= FrameSourceGroupButton_ClickAsync;
        }

        _mediaPlayerElementControl = (MediaPlayerElement)GetTemplateChild(Preview_MediaPlayerElementControl);
        _frameSourceGroupButton = (Button)GetTemplateChild(Preview_FrameSourceGroupButton);

        if (_frameSourceGroupButton != null)
        {
            _frameSourceGroupButton.Click += FrameSourceGroupButton_ClickAsync;
            _frameSourceGroupButton.IsEnabled = false;
            _frameSourceGroupButton.Visibility = Visibility.Collapsed;
        }

        if (_cameraHelper != null)
        {
            await InitializeAsync();
        }
    }

    private async Task InitializeAsync()
    {
        var result = await _cameraHelper.InitializeAndStartCaptureAsync();
        if (result != CameraHelperResult.Success)
        {
            InvokePreviewFailed(result.ToString());
        }

        SetUIControls(result);
    }

    private async void FrameSourceGroupButton_ClickAsync(object sender, RoutedEventArgs e)
    {
        var oldGroup = _cameraHelper.FrameSourceGroup;
        var currentIndex = _frameSourceGroups.Select((grp, index) => new { grp, index }).First(v => v.grp.Id == oldGroup.Id).index;
        var newIndex = currentIndex < (_frameSourceGroups.Count - 1) ? currentIndex + 1 : 0;
        var group = _frameSourceGroups[newIndex];
        _frameSourceGroupButton.IsEnabled = false;
        _cameraHelper.FrameSourceGroup = group;
        await InitializeAsync();

        EventHandler<PreviewCameraChangedEventArgs> handler = PreviewCameraChanged;
        handler?.Invoke(this, new PreviewCameraChangedEventArgs { CameraName = group.DisplayName });
    }

    private void InvokePreviewFailed(string error)
    {
        EventHandler<PreviewFailedEventArgs> handler = PreviewFailed;
        handler?.Invoke(this, new PreviewFailedEventArgs { Error = error });
    }

    private void SetMediaPlayerSource()
    {
        try
        {
            var frameSource = _cameraHelper?.PreviewFrameSource;
            if (frameSource != null)
            {
                if (_mediaPlayer == null)
                {
                    _mediaPlayer = new MediaPlayer
                    {
                        AutoPlay = true,
                        RealTimePlayback = true
                    };
                }

                _mediaPlayer.Source = MediaSource.CreateFromMediaFrameSource(frameSource);
                _mediaPlayerElementControl.SetMediaPlayer(_mediaPlayer);
            }
        }
        catch (Exception ex)
        {
            InvokePreviewFailed(ex.Message);
        }
    }

    private void SetUIControls(CameraHelperResult result)
    {
        var success = result == CameraHelperResult.Success;
        if (success)
        {
            SetMediaPlayerSource();
        }
        else
        {
            _mediaPlayerElementControl.SetMediaPlayer(null);
        }

        _frameSourceGroupButton.IsEnabled = IsFrameSourceGroupButtonAvailable;
        SetFrameSourceGroupButtonVisibility();
    }

    private void SetFrameSourceGroupButtonVisibility()
    {
        _frameSourceGroupButton.Visibility = IsFrameSourceGroupButtonAvailable && IsFrameSourceGroupButtonVisible
                                                            ? Visibility.Visible
                                                            : Visibility.Collapsed;
    }

    /// <summary>
    /// Stops preview.
    /// </summary>
    public void Stop()
    {
        if (_mediaPlayerElementControl != null)
        {
            _mediaPlayerElementControl.SetMediaPlayer(null);
        }

        if (_mediaPlayer != null)
        {
            _mediaPlayer.Dispose();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            _mediaPlayer = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }
    }
}
