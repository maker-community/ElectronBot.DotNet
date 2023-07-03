// Copyright (c) Microsoft Corporation. 
// Licensed under the MIT license. 

using System.Diagnostics;
using CommunityToolkit.WinUI.Helpers;
using ElectronBot.Braincase.Contracts.Services;
using Verdure.ElectronBot.Core.Models;
using ElectronBot.Braincase.Helpers;
using Microsoft.UI.Xaml.Controls;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Storage.Streams;

namespace ElectronBot.Braincase.Services;

class CameraService
{
    private MediaCapture _mediaCapture;

    private List<MediaFrameReader> _sourceReaders = new List<MediaFrameReader>();

    private int _groupSelectionIndex;

    private Image _image;

    public event EventHandler<SoftwareBitmapEventArgs> SoftwareBitmapFrameCaptured;

    private IReadOnlyDictionary<MediaFrameSourceKind, FrameRenderer> _frameRenderers;

    private static CameraService? _current;
    public static CameraService Current => _current ??= new CameraService();

    private CameraHelper? _cameraHelper;
    private bool _isInitialized = false;
    private bool _isProcessing = false;

    private string _cameraName = string.Empty;

    private void RotationHelper_OrientationChanged(object? sender, bool e) => throw new NotImplementedException();

    private void Current_IntelligenceServiceProcessingCompleted(object sender, EventArgs e)
    {
        _isProcessing = false;
    }

    //public async Task<CameraHelperResult> InitializeAsync()
    //{
    //    if (!_isInitialized)
    //    {
    //        var group = await GetFrameSourceGroupAsync();
    //        if (group != null)
    //        {
    //            _cameraHelper = new CameraHelper() { FrameSourceGroup = group };
    //            var result = await _cameraHelper.InitializeAndStartCaptureAsync();
    //            if (result == CameraHelperResult.Success)
    //            {
    //                _cameraHelper.FrameArrived += CameraHelper_FrameArrived;
    //                IntelligenceService.Current.IntelligenceServiceProcessingCompleted += Current_IntelligenceServiceProcessingCompleted;
    //                _isInitialized = true;
    //            }
    //            return result;
    //        }
    //        else
    //        {
    //            return CameraHelperResult.NoFrameSourceGroupAvailable;
    //        }
    //    }
    //    else
    //    {
    //        return CameraHelperResult.Success;
    //    }
    //}

    //private async Task<MediaFrameSourceGroup> GetFrameSourceGroupAsync()
    //{
    //    var availableFrameSourceGroups = await CameraHelper.GetFrameSourceGroupsAsync();
    //    if (availableFrameSourceGroups != null)
    //    {
    //        //get a front-facing camera if one is available
    //        var selectedGroup = availableFrameSourceGroups.Select(group =>
    //           new
    //           {
    //               sourceGroup = group,

    //               colorSourceInfo = group.SourceInfos.FirstOrDefault((sourceInfo) =>
    //               {
    //                   return
    //                       (sourceInfo.MediaStreamType == MediaStreamType.VideoPreview ||
    //                       sourceInfo.MediaStreamType == MediaStreamType.VideoRecord)
    //                   && sourceInfo.SourceKind == MediaFrameSourceKind.Color
    //                   && (sourceInfo.DeviceInformation.EnclosureLocation != null
    //                   && sourceInfo.DeviceInformation.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front);
    //               })
    //           })
    //            .Where(t => t.colorSourceInfo != null)
    //            .FirstOrDefault();

    //        // if we have no front facing camera, take any camera that is available
    //        if (selectedGroup == null)
    //        {
    //            selectedGroup = availableFrameSourceGroups.Select(group =>
    //            new
    //            {
    //                sourceGroup = group,
    //                colorSourceInfo = group.SourceInfos.FirstOrDefault((sourceInfo) =>
    //                {
    //                    return
    //                        (sourceInfo.MediaStreamType == MediaStreamType.VideoPreview ||
    //                        sourceInfo.MediaStreamType == MediaStreamType.VideoRecord)
    //                    && sourceInfo.SourceKind == MediaFrameSourceKind.Color;
    //                })
    //            })
    //            .Where(t => t.colorSourceInfo != null)
    //            .FirstOrDefault();
    //        }
    //        if (selectedGroup != null)
    //        {
    //            return selectedGroup.sourceGroup;
    //        }
    //    }
    //    return null;
    //}

    private void CameraHelper_FrameArrived(object sender, FrameEventArgs e)
    {
        //Debug.WriteLine("A frame arrived in the CameraService");

        if (_isProcessing)
        {
            Debug.WriteLine("A frame already processing in the CameraService.  Throwing away...");
            return;
        }

        // Gets the current video frame
        VideoFrame currentVideoFrame = e.VideoFrame;

        // Gets the software bitmap image
        SoftwareBitmap softwareBitmap = currentVideoFrame.SoftwareBitmap;

        if (currentVideoFrame.SoftwareBitmap != null && !_isProcessing && SoftwareBitmapFrameCaptured != null)
        {
            _isProcessing = true;
            SoftwareBitmapFrameCaptured.Invoke(this, new SoftwareBitmapEventArgs(currentVideoFrame.SoftwareBitmap));
            Debug.WriteLine("The frame should have been sent to the Intelligence Service");
        }
        else if (currentVideoFrame.SoftwareBitmap != null)
        {
            currentVideoFrame.SoftwareBitmap.Dispose();
        }
    }

    public async Task CleanUpAsync()
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException("CameraService not initialized.");
        }

        _cameraHelper.FrameArrived -= CameraHelper_FrameArrived;
        IntelligenceService.Current.IntelligenceServiceProcessingCompleted -= Current_IntelligenceServiceProcessingCompleted;
        _current = null;
        await _cameraHelper.CleanUpAsync();
        _cameraHelper.Dispose();
        _cameraHelper = null;
        _isProcessing = false;
        _isInitialized = false;
    }

    /// <summary>
    /// Switches to the next camera source and starts reading frames.
    /// </summary>
    public async Task PickNextMediaSourceWorkerAsync(Image image)
    {
        // This sample reads three kinds of frames: Color, Depth, and Infrared.
        _frameRenderers = new Dictionary<MediaFrameSourceKind, FrameRenderer>()
        {
            { MediaFrameSourceKind.Color, new FrameRenderer(image) }
        };

        await CleanupMediaCaptureAsync();

        var allGroups = await MediaFrameSourceGroup.FindAllAsync();

        if (allGroups.Count == 0)
        {
            App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                ToastHelper.SendToast("CameraLoadedFailed".GetLocalized(),
                             TimeSpan.FromSeconds(5));
            });
            return;
        }

        var setting = App.GetService<ILocalSettingsService>();

        var saveCamera = await setting.ReadSettingAsync<ComboxItemModel>(Constants.DefaultCameraNameKey);

        MediaFrameSourceGroup selectedGroup = null;

        if (saveCamera != null)
        {
            if (allGroups.Count == 1)
            {
                selectedGroup = allGroups[0];
            }
            else
            {
                var data = allGroups.Where(c => c.DisplayName == saveCamera.DataValue).FirstOrDefault();

                if(data == null)
                {
                    selectedGroup = allGroups[0];
                }
                else
                {
                    selectedGroup = data;
                }
            }        
        }
        else
        {
            var selectedGroup1 = allGroups.Select(group =>
               new
               {
                   sourceGroup = group,
                   colorSourceInfo = group.SourceInfos.FirstOrDefault((sourceInfo) =>
                   {
                       return
                           (sourceInfo.MediaStreamType == MediaStreamType.VideoPreview ||
                           sourceInfo.MediaStreamType == MediaStreamType.VideoRecord)
                       && sourceInfo.SourceKind == MediaFrameSourceKind.Color;
                   })
               })
               .Where(t => t.colorSourceInfo != null)
               .FirstOrDefault();

            if (selectedGroup1 != null)
            {
                selectedGroup = selectedGroup1.sourceGroup;
            }
        }


        try
        {
            // Initialize MediaCapture with selected group.
            // This can raise an exception if the source no longer exists,
            // or if the source could not be initialized.
            await InitializeMediaCaptureAsync(selectedGroup);
        }
        catch (Exception exception)
        {
            await CleanupMediaCaptureAsync();
            return;
        }

        // Set up frame readers, register event handlers and start streaming.
        var startedKinds = new HashSet<MediaFrameSourceKind>();
        foreach (MediaFrameSource source in _mediaCapture.FrameSources.Values)
        {
            MediaFrameSourceKind kind = source.Info.SourceKind;

            // Ignore this source if we already have a source of this kind.
            if (startedKinds.Contains(kind))
            {
                continue;
            }

            // Look for a format which the FrameRenderer can render.
            string requestedSubtype = null;
            foreach (MediaFrameFormat format in source.SupportedFormats)
            {
                requestedSubtype = FrameRenderer.GetSubtypeForFrameReader(kind, format);
                if (requestedSubtype != null)
                {
                    // Tell the source to use the format we can render.
                    await source.SetFormatAsync(format);
                    break;
                }
            }
            if (requestedSubtype == null)
            {
                // No acceptable format was found. Ignore this source.
                continue;
            }

            MediaFrameReader frameReader = await _mediaCapture.CreateFrameReaderAsync(source, requestedSubtype);
            IntelligenceService.Current.IntelligenceServiceProcessingCompleted += Current_IntelligenceServiceProcessingCompleted;
            frameReader.FrameArrived += FrameReader_FrameArrived;
            _sourceReaders.Add(frameReader);

            MediaFrameReaderStartStatus status = await frameReader.StartAsync();
            if (status == MediaFrameReaderStartStatus.Success)
            {
                startedKinds.Add(kind);
            }
            else
            {
            }
        }

        if (startedKinds.Count == 0)
        {
        }

        _isInitialized = true;
    }

    /// <summary>
    /// Initializes the MediaCapture object with the given source group.
    /// </summary>
    /// <param name="sourceGroup">SourceGroup with which to initialize.</param>
    private async Task InitializeMediaCaptureAsync(MediaFrameSourceGroup sourceGroup)
    {
        if (sourceGroup != null)
        {
            _cameraName = sourceGroup.DisplayName;
        }
        if (_mediaCapture != null)
        {
            return;
        }

        // Initialize mediacapture with the source group.
        _mediaCapture = new MediaCapture();
        var settings = new MediaCaptureInitializationSettings
        {
            SourceGroup = sourceGroup,

            // This media capture can share streaming with other apps.
            SharingMode = MediaCaptureSharingMode.SharedReadOnly,

            // Only stream video and don't initialize audio capture devices.
            StreamingCaptureMode = StreamingCaptureMode.Video,

            // Set to CPU to ensure frames always contain CPU SoftwareBitmap images
            // instead of preferring GPU D3DSurface images.
            MemoryPreference = MediaCaptureMemoryPreference.Cpu,

        };

        await _mediaCapture.InitializeAsync(settings);
    }

    /// <summary>
    /// Unregisters FrameArrived event handlers, stops and disposes frame readers
    /// and disposes the MediaCapture object.
    /// </summary>
    public async Task CleanupMediaCaptureAsync()
    {
        if (_mediaCapture != null)
        {
            using (var mediaCapture = _mediaCapture)
            {
                _mediaCapture = null;

                foreach (var reader in _sourceReaders)
                {
                    if (reader != null)
                    {
                        reader.FrameArrived -= FrameReader_FrameArrived;
                        await reader.StopAsync();
                        reader.Dispose();
                    }
                }
                _sourceReaders.Clear();
            }
            IntelligenceService.Current.IntelligenceServiceProcessingCompleted -= Current_IntelligenceServiceProcessingCompleted;
            _isProcessing = false;
        }
    }

    /// <summary>
    /// Handles a frame arrived event and renders the frame to the screen.
    /// </summary>
    private async void FrameReader_FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
    {
        // TryAcquireLatestFrame will return the latest frame that has not yet been acquired.
        // This can return null if there is no such frame, or if the reader is not in the
        // "Started" state. The latter can occur if a FrameArrived event was in flight
        // when the reader was stopped.
        Debug.WriteLine($"1111----in--{DateTime.Now.ToFileTimeUtc()}");

        if (_isProcessing)
        {
            Debug.WriteLine("A frame already processing in the CameraService.  Throwing away...");
            return;
        }
        using (var frame = sender.TryAcquireLatestFrame())
        {
            if (frame != null)
            {
                if (frame.SourceKind == MediaFrameSourceKind.Color)
                {
                    try
                    {
                        var renderer = _frameRenderers[frame.SourceKind];

                        var softwareBitmap1 = FrameRenderer.ConvertToDisplayableImage(frame.VideoMediaFrame);

                        using IRandomAccessStream stream = new InMemoryRandomAccessStream();

                        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

                        encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;

                        if (_cameraName != null && _cameraName.EndsWith("Cam"))
                        {
                            encoder.BitmapTransform.Rotation = BitmapRotation.Clockwise270Degrees;
                            //encoder.BitmapTransform.Rotation = BitmapRotation.Clockwise90Degrees;
                            //encoder.BitmapTransform.Flip = BitmapFlip.Horizontal;
                        }

                        // Set the software bitmap
                        encoder.SetSoftwareBitmap(softwareBitmap1);

                        await encoder.FlushAsync();

                        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

                        var softwareBitmap = await decoder.GetSoftwareBitmapAsync(softwareBitmap1.BitmapPixelFormat, softwareBitmap1.BitmapAlphaMode);

                        var copySoftwareBitmap = SoftwareBitmap.Copy(softwareBitmap);

                        renderer.RenderFrame(copySoftwareBitmap);

                        if (softwareBitmap != null && !_isProcessing && SoftwareBitmapFrameCaptured != null)
                        {
                            _isProcessing = true;
                            SoftwareBitmapFrameCaptured.Invoke(this, new SoftwareBitmapEventArgs(softwareBitmap));
                            Debug.WriteLine("The frame should have been sent to the Intelligence Service");
                        }
                        else if (softwareBitmap != null)
                        {
                            softwareBitmap.Dispose();
                        }
                        softwareBitmap1.Dispose();
                    }
                    catch (Exception)
                    {

                    }

                }
            }
        }
    }
}
