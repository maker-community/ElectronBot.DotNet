using ElectronBot.Braincase.Core.Models;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Microsoft.Graph;
using Verdure.ElectronBot.Core.Models;
using Verdure.ElectronBot.GrpcService;

namespace ElectronBot.Braincase.Services.EbotGrpcService;
public class EbGrpcService
{
    private readonly ElectronBotActionGrpc.ElectronBotActionGrpcClient _client;
    public EbGrpcService(ElectronBotActionGrpc.ElectronBotActionGrpcClient client)
    {
        _client = client;
    }

    public async Task<string> PlayEmotionActionFrameAsync(EmoticonActionFrame frame)
    {
        var data = new EmotionActionFrameRequest
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
        var result = await _client.PlayEmotionActionAsync(data);

        return result.Message;
    }


    public async Task<string> PlayEmotionActionFramesAsync(List<EmoticonActionFrame> frame)
    {
        var dataList = new RepeatedField<EmotionActionFrameRequest>();

        if (frame != null && frame.Count > 0)
        {
            foreach (var itemFrame in frame)
            {
                var data = new EmotionActionFrameRequest
                {
                    FrameBuffer = ByteString.CopyFrom(itemFrame.FrameBuffer),

                    Enable = itemFrame.Enable,
                    J1 = itemFrame.J1,
                    J2 = itemFrame.J2,
                    J3 = itemFrame.J3,
                    J4 = itemFrame.J4,
                    J5 = itemFrame.J5,
                    J6 = itemFrame.J6
                };
                dataList.Add(data);
            }
        }

        var emoticonActionFrameRequest = new EmotionActionFramesRequest();

        emoticonActionFrameRequest.ActionsRequest.AddRange(dataList);

        var result = await _client.PlayEmoitonActionsAsync(emoticonActionFrameRequest);

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

