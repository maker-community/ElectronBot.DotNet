using System.Device.Gpio;
using ElectronBot.DotNet;
using Grpc.Core;
using Verdure.ElectronBot.Core.Models;

namespace Verdure.ElectronBot.GrpcService.Services;
public class ElectronBotActionService : ElectronBotActionGrpc.ElectronBotActionGrpcBase
{
    private readonly ILogger<ElectronBotActionService> _logger;

    private readonly IElectronLowLevel _electronLowLevel;

    private readonly int pin5 = 5;
    private readonly int pin6 = 6;
    private readonly int pin13 = 13;
    private readonly int pin19 = 19;
    public ElectronBotActionService(ILogger<ElectronBotActionService> logger,
        IElectronLowLevel electronLowLevel)
    {
        _logger = logger;
        _electronLowLevel = electronLowLevel;
    }

    public override Task<EbHelloReply> PlayEmotionAction(
        EmotionActionFrameRequest request, ServerCallContext context)
    {
        var frameAction = request.FrameBuffer.ToByteArray();

        var actionData = new EmoticonActionFrame(frameAction, request.Enable,
            request.J1,
            request.J2,
            request.J3,
            request.J4,
            request.J5,
            request.J6);

        EmojiPlayHelper.Current.Enqueue(actionData);

        var result = new EbHelloReply() { Message = "ok" };

        return Task.FromResult(result);
    }

    public override Task<EbHelloReply> PlayEmoitonActions(EmotionActionFramesRequest request, ServerCallContext context)
    {
        if (request.ActionsRequest != null && request.ActionsRequest.Count > 0)
        {
            foreach (var frameItem in request.ActionsRequest)
            {
                var frameAction = frameItem.FrameBuffer.ToByteArray();

                var actionData = new EmoticonActionFrame(frameAction, frameItem.Enable,
                    frameItem.J1,
                    frameItem.J2,
                    frameItem.J3,
                    frameItem.J4,
                    frameItem.J5,
                    frameItem.J6);
                Task.Run(() =>
                {
                    if (EmojiPlayHelper.Current.ElectronLowLevel.IsConnected)
                    {
                        EmojiPlayHelper.Current.ElectronLowLevel.SetImageSrc(actionData.FrameBuffer);
                        EmojiPlayHelper.Current.ElectronLowLevel.SetJointAngles(actionData.J1, actionData.J2, actionData.J3, actionData.J4, actionData.J5, actionData.J6, actionData.Enable);
                        EmojiPlayHelper.Current.ElectronLowLevel.Sync();
                    }
                });

            }
        }
        var result = new EbHelloReply() { Message = "ok" };

        return Task.FromResult(result);
    }

    public override Task<EbHelloReply> SendMotorControl(MotorControlRequest request, ServerCallContext context)
    {
        var init1 = request.Init1;
        var init2 = request.Init2;
        var init3 = request.Init3;
        var init4 = request.Init4;

        using GpioController controller = new();
        controller.OpenPin(pin5, PinMode.Output);
        controller.OpenPin(pin6, PinMode.Output);
        controller.OpenPin(pin13, PinMode.Output);
        controller.OpenPin(pin19, PinMode.Output);

        controller.Write(pin5, ConvertPinValue(init1));
        controller.Write(pin6, ConvertPinValue(init2));
        controller.Write(pin13, ConvertPinValue(init3));
        controller.Write(pin19, ConvertPinValue(init4));


        _logger.LogInformation($"init1---{init1}");

        _logger.LogInformation($"init2---{init2}");

        _logger.LogInformation($"init3---{init3}");

        _logger.LogInformation($"init4---{init4}");

        var result = new EbHelloReply() { Message = "ok" };

        return Task.FromResult(result);
    }

    public PinValue ConvertPinValue(int init)
    {
        if (init == 0)
        {
            return PinValue.Low;
        }
        return PinValue.High;
    }
}
