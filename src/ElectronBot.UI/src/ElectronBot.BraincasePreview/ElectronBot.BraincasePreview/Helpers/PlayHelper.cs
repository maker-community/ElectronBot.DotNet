using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using ElectronBot.DotNet;
using ElectronBot.BraincasePreview.Core.Models;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace ElectronBot.BraincasePreview.Helpers;
public class PlayHelper
{
    private static PlayHelper _current;
    public static PlayHelper Current => _current ??= new PlayHelper();

    private Queue<SoftwareBitmap> softwareBitmaps = new();

    private IElectronLowLevel _electron;

    public void Start()
    {
        _electron = ElectronBotHelper.Instance.ElectronBot;


        Thread thread = new Thread(RunPlayAsync);

        thread.IsBackground = true;
        thread.Start();

    }

    private async void RunPlayAsync()
    {
        while (true)
        {
            try
            {
                if (softwareBitmaps.Count > 5)
                {
                    var softwareBitmap = softwareBitmaps.Dequeue();

                    if (softwareBitmap != null)
                    {
                        using IRandomAccessStream stream = new InMemoryRandomAccessStream();

                        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

                        // Set the software bitmap
                        encoder.SetSoftwareBitmap(softwareBitmap);

                        await encoder.FlushAsync();

                        var image = new Bitmap(stream.AsStream());

                        var mat = OpenCvSharp.Extensions.BitmapConverter.ToMat(image);

                        var mat1 = mat.Resize(new OpenCvSharp.Size(240, 240), 0, 0, OpenCvSharp.InterpolationFlags.Area);

                        var mat2 = mat1.CvtColor(OpenCvSharp.ColorConversionCodes.RGBA2BGR);

                        var dataMeta = mat2.Data;

                        var data = new byte[240 * 240 * 3];

                        Marshal.Copy(dataMeta, data, 0, 240 * 240 * 3);

                        if (ElectronBotHelper.Instance.EbConnected)
                        {
                            try
                            {
                                if (ElectronBotHelper.Instance.EbConnected)
                                {
                                    var frame = new EmoticonActionFrame(data);

                                    ElectronBotHelper.Instance.PlayEmoticonActionFrame(frame);
                                }
                            }
                            catch (Exception ex)
                            {

                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }
    }

    public void Enqueue(SoftwareBitmap bitmap)
    {
        softwareBitmaps.Enqueue(bitmap);
    }
}
