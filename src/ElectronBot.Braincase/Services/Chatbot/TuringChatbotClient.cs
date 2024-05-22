using System.Net.Http.Json;
using System.Text.Json;
using Contracts.Services;
using ElectronBot.Braincase;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using Models;

namespace Services;
public class TuringChatbotClient : IChatbotClient
{
    public string Name => "Turing";

    private const string BaseUrl = "http://openapi.turingapi.com/openapi/api/v2";

    private readonly IHttpClientFactory _httpClientFactory;

    private readonly ILocalSettingsService _localSettingsService;
    public TuringChatbotClient(IHttpClientFactory httpClientFactory,
        ILocalSettingsService localSettingsService)
    {
        _httpClientFactory = httpClientFactory;
        _localSettingsService = localSettingsService;
    }


    public async Task<string> AskQuestionResultAsync(string message)
    {
        var result = await _localSettingsService.ReadSettingAsync<BotSetting>(Constants.BotSettingKey);
        if (result == null)
        {
            throw new Exception("配置为空");
        }
        var requestData = new TuringRequest
        {
            Perception = new Perception
            {
                InputText = new Inputtext
                {
                    Text = message
                }
            },
            UserInfo = new Userinfo
            {
                UserId = result.TuringUserId,
                ApiKey = result.TuringAppkey
            }
        };

        try
        {
            var httpClient = _httpClientFactory.CreateClient();

            var turingRet = await httpClient.PostAsJsonAsync(BaseUrl, requestData);

            if (turingRet.IsSuccessStatusCode)
            {
                var retContent = await turingRet.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var retObj = System.Text.Json.JsonSerializer.Deserialize<TuringResultDto>(retContent, options);

                if (retObj is not null)
                {
                    if (retObj.Results != null && retObj.Results.Count > 0)
                    {
                        var retText = retObj.Results[0].Values.Text;
                        
                        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
                        {
                            ToastHelper.SendToast(retText, TimeSpan.FromSeconds(5));
                        });
                        return retText;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

        return "";
    }
}


public class TuringRequest
{
    public int ReqType
    {
        get; set;
    } = 0;
    public Perception Perception
    {
        get; set;
    } = new();
    public Userinfo UserInfo
    {
        get; set;
    } = new();
}

public class Perception
{
    public Inputtext InputText
    {
        get; set;
    } = new();
}

public class Inputtext
{
    public string Text
    {
        get; set;
    } = string.Empty;
}

public class Userinfo
{
    public string ApiKey
    {
        get; set;
    } = string.Empty;
    public string UserId
    {
        get; set;
    } = string.Empty;
}




public class TuringResultDto
{
    public Emotion Emotion
    {
        get; set;
    } = new();
    public Intent Intent
    {
        get; set;
    } = new();
    public List<Result> Results
    {
        get; set;
    } = new();
}

public class Emotion
{
    public Robotemotion RobotEmotion
    {
        get; set;
    } = new();
    public Useremotion UserEmotion
    {
        get; set;
    } = new();
}

public class Robotemotion
{
    public int A
    {
        get; set;
    }
    public int D
    {
        get; set;
    }
    public int EmotionId
    {
        get; set;
    }
    public int P
    {
        get; set;
    }
}

public class Useremotion
{
    public int A
    {
        get; set;
    }
    public int D
    {
        get; set;
    }
    public int EmotionId
    {
        get; set;
    }
    public int P
    {
        get; set;
    }
}

public class Intent
{
    public string AppKey
    {
        get; set;
    } = string.Empty;
    public int Code
    {
        get; set;
    }
    public int OperateState
    {
        get; set;
    }
}

public class Result
{
    public int GroupType
    {
        get; set;
    }
    public string ResultType
    {
        get; set;
    } = string.Empty;
    public Values Values
    {
        get; set;
    } = new();
}

public class Values
{
    public string Text
    {
        get; set;
    } = string.Empty;
}

