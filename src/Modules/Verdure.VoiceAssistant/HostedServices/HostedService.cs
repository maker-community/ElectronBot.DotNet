using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using NetCoreAudio;
using Verdure.Braincase.Core.Contracts.Services;
using Verdure.Braincase.Helpers;
using Windows.ApplicationModel;

namespace Verdure.VoiceAssistant.HostedServices;

/// <summary>
/// A hosted service providing the primary conversation loop for Semantic Kernel with OpenAI ChatGPT.
/// </summary>
public class HostedService : IHostedService, IDisposable
{
    private readonly ILogger<HostedService> _logger;

    private readonly IWakeWordListener _wakeWordListener;

    private readonly DispatcherQueue _dispatcherQueue;

    private Task _executeTask;
    private readonly CancellationTokenSource _cancelToken = new();

    // Notification sound support
    private readonly string _notificationSoundFilePath;
    private readonly Player _player;

    /// <summary>
    /// Constructor
    /// </summary>
    public HostedService(IWakeWordListener wakeWordListener,
        ILogger<HostedService> logger)
    {
        _logger = logger;
        _wakeWordListener = wakeWordListener;
        _notificationSoundFilePath = Package.Current.InstalledLocation.Path + $"\\Assets\\Keyword\\bing.mp3";
        _player = new Player();
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
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
            // Play a notification to let the user know we have started listening for the wake phrase.
            await _player.Play(_notificationSoundFilePath);

            // Wait for wake word or phrase
            if (!await _wakeWordListener.WaitForWakeWordAsync(cancellationToken))
            {
                continue;
            }

            await _player.Play(_notificationSoundFilePath);

            // Say hello on startup

            // Start listening
            while (!cancellationToken.IsCancellationRequested)
            {
                // Listen to the user
                var userSpoke = string.Empty;//context.Result;

                // Wait for wake word or phrase
                if (!await _wakeWordListener.WaitForWakeWordAsync(cancellationToken))
                {
                    //continue;
                }

                await _player.Play(_notificationSoundFilePath);
                // Get a reply from the AI and add it to the chat history.
                var reply = string.Empty;
                try
                {
                    _dispatcherQueue.TryEnqueue(() =>
                    {
                        ToastHelper.SendToast("keyword is ok", TimeSpan.FromSeconds(3));

                    });
                }
                catch (Exception aiex)
                {
                    _logger.LogError($"OpenAI returned an error.{aiex.Message}");
                    reply = "OpenAI returned an error. Please try again.";
                }

                // Speak the AI's reply

                // If the user said "Goodbye" - stop listening and wait for the wake work again.
                if (userSpoke.StartsWith("goodbye", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }
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