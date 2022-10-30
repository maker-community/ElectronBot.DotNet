using ElectronBot.BraincasePreview.Core.Models;
using Google.Protobuf;
using Verdure.ElectronBot.Core.Models;
using Verdure.ElectronBot.GrpcService;

namespace ElectronBot.BraincasePreview.Services.EbotGrpcService;
public class EbGrpcService
{
    private readonly ElectronBotActionGrpc.ElectronBotActionGrpcClient _client;
    public EbGrpcService(ElectronBotActionGrpc.ElectronBotActionGrpcClient client)
    {
        _client = client;
    }

    public async Task<string> PlayEmoticonActionFrameAsync(EmoticonActionFrame frame)
    {
        var data = new EmoticonActionFrameRequest
        {
            FrameBuffer = ByteString.CopyFrom(frame.FrameBuffer),

            Enable = frame.Enable,
            J1 = frame.J1,
            J2 = frame.J2,
            J3 = frame.J3,
            J4 = frame.J4,
            J5 = frame.J5,
            J6 = frame.J6
        };
        var result = await _client.PlayEmoticonActionAsync(data);

        return result.Message;
    }


    public async Task<string> MotorControlAsync(MotorControlRequestModel requestModel)
    {
        var data = new MotorControlRequest
        {
            Init1 = requestModel.Init1,
            Init2 = requestModel.Init2,
            Init3 = requestModel.Init3,
            Init4 = requestModel.Init4,
            EnableA = requestModel.EnableA,
            EnableB = requestModel.EnableB
        };

        var result = await _client.SendMotorControlAsync(data);

        return result.Message;
    }
}

