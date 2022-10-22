using ElectronBot.DotNet;
using Grpc.Core;
using Verdure.ElectronBot.Core.Models;

namespace Verdure.ElectronBot.GrpcService.Services;
public class ElectronBotActionService : ElectronBotActionGrpc.ElectronBotActionGrpcBase
{
    private readonly ILogger<ElectronBotActionService> _logger;

    private readonly IElectronLowLevel _electronLowLevel;
    public ElectronBotActionService(ILogger<ElectronBotActionService> logger, IElectronLowLevel electronLowLevel)
    {
        _logger = logger;
        _electronLowLevel = electronLowLevel;
    }

    public override Task<EbHelloReply> PlayEmoticonAction(
        EmoticonActionFrameRequest request, ServerCallContext context)
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

    public override Task<EbHelloReply> SendMotorControl(MotorControlRequest request, ServerCallContext context)
    {
        var result = new EbHelloReply() { Message = "ok" };

        return Task.FromResult(result);
    }
}
