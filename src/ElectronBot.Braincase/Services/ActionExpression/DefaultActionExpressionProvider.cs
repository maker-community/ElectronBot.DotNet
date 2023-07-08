using System.Runtime.InteropServices;
using System.Text.Json;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using ElectronBot.Braincase.Services.EbotGrpcService;
using OpenCvSharp;
using Services;
using Verdure.ElectronBot.Core.Models;
using Windows.ApplicationModel;

namespace ElectronBot.Braincase.Services;
public class DefaultActionExpressionProvider : IActionExpressionProvider
{
    public string Name => "Default";

    public async Task PlayActionExpressionAsync(string actionName)
    {
        Mat image = new();

        var capture = new VideoCapture(Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\{actionName}.mp4");

        var service = App.GetService<EmoticonActionFrameService>();

        service.ClearQueue();

        while (true)
        {
            capture.Read(image);

            capture.Set(OpenCvSharp.VideoCaptureProperties.PosFrames,
                capture.Get(OpenCvSharp.VideoCaptureProperties.PosFrames) + 1);

            if (image.Empty())
            {
                break;
            }
            else
            {

                //var mat1 = image.Resize(new OpenCvSharp.Size(240, 240), 0, 0, OpenCvSharp.InterpolationFlags.Lanczos4);

                //var mat2 = mat1.CvtColor(OpenCvSharp.ColorConversionCodes.RGBA2BGR);

                var dataMeta = image.Data;

                var data = new byte[240 * 240 * 3];

                Marshal.Copy(dataMeta, data, 0, 240 * 240 * 3);

                //EmojiPlayHelper.Current.Enqueue(new EmoticonActionFrame(data));

                await service.SendToUsbDeviceAsync(new EmoticonActionFrame(data));
            }
        }
    }

    public async Task PlayActionExpressionAsync(string actionName, List<ElectronBotAction> actions)
    {
        Mat image = new();

        var capture = new VideoCapture(Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\{actionName}.mp4");

        var frameCount = capture.FrameCount;

        var currentAction = new ElectronBotAction();

        var service = App.GetService<EmoticonActionFrameService>();

        service.ClearQueue();

        while (true)
        {
            capture.Read(image);

            if (ElectronBotHelper.Instance.EbConnected)
            {
                capture.Set(OpenCvSharp.VideoCaptureProperties.PosFrames,
                    capture.Get(OpenCvSharp.VideoCaptureProperties.PosFrames) + 1);

            }

            if (image.Empty())
            {
                break;
            }
            else
            {
                if (actions != null && actions.Count > 0)
                {
                    var pos = capture.PosFrames;

                    var bili = ((double)pos / frameCount);

                    var actionCount = (int)(actions.Count * bili);

                    if (actionCount >= actions.Count)
                    {
                        actionCount = actions.Count - 1;
                    }


                    currentAction = actions[actionCount];
                }

                //var mat1 = image.Resize(new OpenCvSharp.Size(240, 240), 0, 0, OpenCvSharp.InterpolationFlags.Lanczos4);

                //var mat2 = mat1.CvtColor(OpenCvSharp.ColorConversionCodes.RGBA2BGR);

                var dataMeta = image.Data;

                var data = new byte[240 * 240 * 3];

                Marshal.Copy(dataMeta, data, 0, 240 * 240 * 3);

                var frameData = new EmoticonActionFrame(data, true,
                    currentAction.J1,
                    currentAction.J2,
                    currentAction.J3,
                    currentAction.J4,
                    currentAction.J5,
                    currentAction.J6);

                //EmojiPlayHelper.Current.Enqueue(frameData);


                if (!ElectronBotHelper.Instance.EbConnected)
                {
                    await Task.Delay(30);
                }
                _ = await service.SendToUsbDeviceAsync(frameData);

                //通过grpc通讯和树莓派传输数据 
                //var grpcClient = App.GetService<EbGrpcService>();

                //await grpcClient.PlayEmoticonActionFrameAsync(frameData);
            }
        }
    }

    public async Task PlayActionExpressionAsync(EmoticonAction emoticonAction, List<ElectronBotAction> actions)
    {
        Mat image = new();

        VideoCapture? capture;

        string? path;

        if (emoticonAction.EmojisType == EmojisType.Default)
        {
            path = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\{emoticonAction.NameId}.mp4";
        }
        else
        {
            path = emoticonAction.EmojisVideoPath;   
        }
        capture = new VideoCapture(path);

        var frameCount = capture.FrameCount;

        var currentAction = new ElectronBotAction();

        var service = App.GetService<EmoticonActionFrameService>();

        service.ClearQueue();

        while (true)
        {
            capture.Read(image);

            if (ElectronBotHelper.Instance.EbConnected)
            {
                capture.Set(OpenCvSharp.VideoCaptureProperties.PosFrames,
                    capture.Get(OpenCvSharp.VideoCaptureProperties.PosFrames) + 1);
            }
         

            if (image.Empty())
            {
                break;
            }
            else
            {
                if (actions != null && actions.Count > 0)
                {
                    var pos = capture.PosFrames;

                    var bili = ((double)pos / frameCount);

                    var actionCount = (int)(actions.Count * bili);

                    if (actionCount >= actions.Count)
                    {
                        actionCount = actions.Count - 1;
                    }


                    currentAction = actions[actionCount];
                }

               

                var mat1 = image.Resize(new OpenCvSharp.Size(240, 240), 0, 0, OpenCvSharp.InterpolationFlags.Lanczos4);

                var mat2 = mat1.CvtColor(OpenCvSharp.ColorConversionCodes.RGBA2BGR);

                var dataMeta = mat2.Data;

                var data = new byte[240 * 240 * 3];

                Marshal.Copy(dataMeta, data, 0, 240 * 240 * 3);

                var frameData = new EmoticonActionFrame(data, true,
                    currentAction.J1,
                    currentAction.J2,
                    currentAction.J3,
                    currentAction.J4,
                    currentAction.J5,
                    currentAction.J6);

                var stream = image.ToMemoryStream();

                var modelFrameData = new ModelActionFrame(stream, true,
                    currentAction.J1,
                    currentAction.J2,
                    currentAction.J3,
                    currentAction.J4,
                    currentAction.J5,
                    currentAction.J6);

                //EmojiPlayHelper.Current.Enqueue(frameData);
                ElectronBotHelper.Instance.ModelActionInvoke(modelFrameData);

                if(!ElectronBotHelper.Instance.EbConnected)
                {
                    await Task.Delay(30);
                }
                _ = await service.SendToUsbDeviceAsync(frameData);

                //通过grpc通讯和树莓派传输数据 
                //var grpcClient = App.GetService<EbGrpcService>();

                //await grpcClient.PlayEmoticonActionFrameAsync(frameData);
            }
        }
        ElectronBotHelper.Instance.PlayEmojisLock = false;
    }

    public async Task PlayActionExpressionAsync(EmoticonAction emoticonAction)
    {
        Mat image = new();

        VideoCapture? capture;

        if (emoticonAction.EmojisType == EmojisType.Default)
        {
            capture = new VideoCapture(Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\{emoticonAction.NameId}.mp4");
        }
        else
        {
            capture = new VideoCapture(emoticonAction.EmojisVideoPath);
        }

        var frameCount = capture.FrameCount;

        var currentAction = new ElectronBotAction();

        List<ElectronBotAction> actions = new();

        if (emoticonAction.HasAction)
        {
            if (!string.IsNullOrWhiteSpace(emoticonAction.EmojisActionPath))
            {
                var actionJson = emoticonAction.EmojisActionPath;
                if (emoticonAction.EmojisActionPath == "defaultaction.json")
                {
                    actionJson = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\defaultaction.json";
                }
                var json = File.ReadAllText(actionJson);
                //var json = File.ReadAllText(emoticonAction.EmojisActionPath);

                try
                {
                    var actionList = JsonSerializer.Deserialize<List<ElectronBotAction>>(json);

                    if (actionList != null && actionList.Count > 0)
                    {
                        actions = actionList;
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        //var actionsList = new List<EmoticonActionFrame>();

        var service = App.GetService<EmoticonActionFrameService>();

        service.ClearQueue();

        while (true)
        {
            if (capture != null)
            {
                try
                {
                    capture.Read(image);

                    capture.Set(OpenCvSharp.VideoCaptureProperties.PosFrames,
                        capture.Get(OpenCvSharp.VideoCaptureProperties.PosFrames) + 4);

                    if (image.Empty())
                    {
                        break;
                    }
                    else
                    {
                        if (actions != null && actions.Count > 0)
                        {
                            var pos = capture.PosFrames;

                            var bili = ((double)pos / frameCount);

                            var actionCount = (int)(actions.Count * bili);

                            if (actionCount >= actions.Count)
                            {
                                actionCount = actions.Count - 1;
                            }


                            currentAction = actions[actionCount];
                        }

                        var mat1 = image.Resize(new OpenCvSharp.Size(240, 240), 0, 0, OpenCvSharp.InterpolationFlags.Lanczos4);

                        var mat2 = mat1.CvtColor(OpenCvSharp.ColorConversionCodes.RGBA2BGR);

                        var dataMeta = mat2.Data;

                        var data = new byte[240 * 240 * 3];

                        Marshal.Copy(dataMeta, data, 0, 240 * 240 * 3);

                        var frameData = new EmoticonActionFrame(data, true,
                            currentAction.J1,
                            currentAction.J2,
                            currentAction.J3,
                            currentAction.J4,
                            currentAction.J5,
                            currentAction.J6);

                        _ = await service.SendToUsbDeviceAsync(frameData);
                        //actionsList.Add(frameData);

                        //通过grpc通讯和树莓派传输数据 
                        //var grpcClient = App.GetService<EbGrpcService>();

                        //await grpcClient.PlayEmotionActionFrameAsync(frameData);
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        //var grpcClient = App.GetService<EbGrpcService>();

        //await grpcClient.PlayEmotionActionFramesAsync(actionsList);
    }
}
