using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Helpers;
using ElectronBot.BraincasePreview.Contracts.Services;
using ElectronBot.BraincasePreview.Core.Models;
using ElectronBot.BraincasePreview.Services;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Windows.Graphics.Imaging;

namespace ElectronBot.BraincasePreview.Helpers;
public class OpenCvCameraHelper : IDisposable
{
    private bool flag = false;

    private VideoCapture video = new(0);

    private ComboxItemModel _saveCamera;

    public event EventHandler<SoftwareBitmapEventArgs> FrameArrived;
    public OpenCvCameraHelper()
    {
    }


    public async Task InitializeCameraAsync()
    {
        var setting = App.GetService<ILocalSettingsService>();

        var saveCamera = await setting.ReadSettingAsync<ComboxItemModel>(Constants.DefaultCameraNameKey);

        if (saveCamera != null)
        {
            _saveCamera = saveCamera;

            video = new VideoCapture(Convert.ToInt32(saveCamera.DataKey));
        }
        else
        {
            video = new VideoCapture(0);
        }
    }

    public Task<CameraHelperResult> InitializeAndStartCaptureAsync(CancellationToken cancellationToken = default)
    {
        flag = true;

        Task.Run(async () =>
        {
            await RunCapAsync();

        }, cancellationToken);

        return Task.FromResult(CameraHelperResult.Success);
    }
    async Task RunCapAsync()
    {
        var src = new Mat();

        if (!video.IsOpened())
        {
            Console.WriteLine("摄像头打开失败");

            var text = "CameraErrorText".GetLocalized();

            App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                ToastHelper.SendToast(text, TimeSpan.FromSeconds(5));
            });
            

            return;
        }
        else
        {
            while (true)
            {
                if (flag)
                {
                    try
                    {
                        video.Read(src);

                        if (!src.Empty())//读取视频文件时,判定帧是否为空,如果帧为空,则下方的图片处理会报异常
                        {
                            var src1 = new Mat();
                            var src2 = new Mat();

                            Bitmap bitmap = BitmapConverter.ToBitmap(src);

                            if (_saveCamera != null)
                            {
                                if (_saveCamera.DataValue.Contains(Constants.DefaultCameraName))
                                {
                                    Cv2.Flip(src, src1, FlipMode.Y); //先翻转

                                    Cv2.Rotate(src1, src2, RotateFlags.Rotate90Clockwise); //再旋转

                                    bitmap = BitmapConverter.ToBitmap(src2);
                                }
                            }

                            var ret = await BitmapToBitmapImage(bitmap);

                            FrameArrived?.Invoke(this, new SoftwareBitmapEventArgs(ret));
                        }
                    }
                    catch(Exception ex)
                    {

                    }              
                }
                else
                {
                    video.Release();

                    break;
                }

            }
        }
    }

    public async Task<SoftwareBitmap> BitmapToBitmapImage(System.Drawing.Bitmap bitmap)
    {
        MemoryStream ms = new MemoryStream();

        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

        ms.Seek(0, SeekOrigin.Begin);

        // Create the decoder from the stream
        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(ms.AsRandomAccessStream());

        // Get the SoftwareBitmap representation of the file
        var softwareBitmap = await decoder.GetSoftwareBitmapAsync();

        return softwareBitmap;
    }

    public void CleanUp()
    {
        try
        {
            StopReader();
        }
        finally
        {
        }
    }

    private void StopReader()
    {
        video.Release();

        flag = false;
    }

    public void Dispose()
    {
        flag = false;
    }
}
