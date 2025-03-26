using BotSharp.Abstraction.Agents.Enums;
using BotSharp.Abstraction.Conversations;
using BotSharp.Abstraction.Conversations.Models;
using BotSharp.Abstraction.Routing;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Models;
using NetCoreAudio;
using Verdure.Braincase.Core.Contracts.Services;
using Verdure.Braincase.Helpers;
using Verdure.Braincase.WinUI.Common;
using Verdure.VoiceAssistant.Services;
using Windows.ApplicationModel;

namespace Verdure.VoiceAssistant.HostedServices;

/// <summary>
/// A hosted service providing the primary conversation loop for Semantic Kernel with OpenAI ChatGPT.
/// </summary>
public class HostedService : IHostedService, IDisposable
{
    private readonly ILogger<HostedService> _logger;

    private readonly IWakeWordListener _wakeWordListener;

    private readonly ILocalSettingsService _localSettingsService;

    private readonly IConversationService _conversationService;

    private readonly IRoutingService _routing;

    private readonly IServiceProvider _serviceProvider;

    private readonly DispatcherQueue _dispatcherQueue;

    private readonly IElectronBotPlayer _electronBotPlayer;

    private Task _executeTask;
    private readonly CancellationTokenSource _cancelToken = new();

    // Notification sound support
    private readonly string _notificationSoundFilePath;
    private readonly Player _player;

    private BotSetting? _botSetting;

    /// <summary>
    /// Constructor
    /// </summary>
    public HostedService(IWakeWordListener wakeWordListener,
        ILogger<HostedService> logger,
        IBotSpeech botSpeech,
        ILocalSettingsService localSettingsService,
        IConversationService conversationService,
        IRoutingService routing,
        IServiceProvider serviceProvider,
        IElectronBotPlayer electronBotPlayer)
    {
        _logger = logger;
        _wakeWordListener = wakeWordListener;
        _notificationSoundFilePath = Package.Current.InstalledLocation.Path + $"\\Assets\\Keyword\\bing.mp3";
        _player = new Player();
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _localSettingsService = localSettingsService;
        _conversationService = conversationService;
        _routing = routing;
        _serviceProvider = serviceProvider;
        _electronBotPlayer = electronBotPlayer;
    }

    /// <summary>
    /// Start the service.
    /// </summary>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _executeTask = ExecuteAsync(_cancelToken.Token);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Primary service logic loop.
    /// </summary>
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _botSetting = await _localSettingsService.ReadSettingAsync<BotSetting>(Constants.BotSettingKey);
                // Play a notification to let the user know we have started listening for the wake phrase.
                await _player.Play(_notificationSoundFilePath);

                var botSpeech = await BotSpeechProvider.GetBotSpeechAsync(_serviceProvider);

                await botSpeech.InitAsync(cancellationToken);
                // Wait for wake word or phrase
                if (!await _wakeWordListener.WaitForWakeWordAsync(cancellationToken))
                {
                    continue;
                }

                await _player.Play(_notificationSoundFilePath);

                var helloString = _botSetting?.AnswerText;
                // Say hello on startup
                await botSpeech.SpeakAsync(helloString ?? "Hello!", cancellationToken);
                // Start listening
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Listen to the user
                    var userSpoke = await botSpeech.ListenAsync(cancellationToken);

                    _dispatcherQueue.TryEnqueue(() =>
                    {
                        ToastHelper.SendToast($"用户的问题:{userSpoke}", TimeSpan.FromSeconds(3));
                    });
                    // Get a reply from the AI and add it to the chat history.
                    var reply = string.Empty;


                    var saveConv = await _localSettingsService
                        .ReadSettingAsync<Conversation>(Constants.CurrentConversationKey);

                    if (saveConv == null)
                    {
                        _logger.LogError("No conversation ID found.");
                        continue;
                    }

                    var inputMsg = new RoleDialogModel(AgentRole.User, userSpoke)
                    {
                        MessageId = Guid.NewGuid().ToString(),
                        CreatedAt = DateTime.UtcNow
                    };

                    WeakReferenceMessenger.Default.Send(inputMsg);

                    _routing.Context.SetMessageId(saveConv.Id, inputMsg.MessageId);

                    _conversationService.SetConversationId(saveConv.Id, new());

                    try
                    {
                        // 启动动画但不阻塞当前执行流程
                        var animationTask = _electronBotPlayer.PlayLottieByNameIdAsync("think", -1);

                        // 可以选择添加异常处理
                        animationTask?.ContinueWith(t =>
                        {
                            if (t.IsFaulted)
                            {
                                _logger.LogError($"Animation playback failed: {t.Exception}");
                            }
                        }, TaskContinuationOptions.OnlyOnFaulted);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Failed to start animation: {ex.Message}");
                        await _electronBotPlayer.StopLottiePlaybackAsync();
                        // 根据需要处理异常
                    }

                    await Task.Run(async () =>
                    {
                        await _conversationService.SendMessage(saveConv.AgentId, inputMsg,
                            replyMessage: null,
                            async msg =>
                            {
                                reply = msg.Content;
                                _dispatcherQueue.TryEnqueue(() =>
                                {
                                    WeakReferenceMessenger.Default.Send(msg);
                                    ToastHelper.SendToast($"result:{msg.Content}", TimeSpan.FromSeconds(3));
                                });
                            });
                    });

                    await _electronBotPlayer.StopLottiePlaybackAsync();
                    // Speak the AI's reply
                    await botSpeech.SpeakAsync(reply, cancellationToken);
                   
                    // If the user said "Goodbye" - stop listening and wait for the wake work again.
                    if (userSpoke.StartsWith("再见") || userSpoke.StartsWith("goodbye", StringComparison.InvariantCultureIgnoreCase))
                    {
                        break;
                    }
                }
            }
            catch (Exception aiex)
            {
                _logger.LogError($"OpenAI returned an error.{aiex.Message}");
                ToastHelper.SendToast("LLM API KEY OR VOICE API KEY MAY NOT OK", TimeSpan.FromSeconds(3));
            }
        }
    }

    /// <summary>
    /// Stop a running service.
    /// </summary>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cancelToken.Cancel();
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual void Dispose()
    {
        _cancelToken.Dispose();
        _wakeWordListener.Dispose();
    }
}