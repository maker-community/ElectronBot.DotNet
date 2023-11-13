using System.Diagnostics;
using Contracts.Services;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Helpers;
using Models;
using Verdure.ElectronBot.Core.Models;
using Windows.Globalization;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;
using Windows.System;

namespace ElectronBot.Braincase.Services;
public class SpeechAndTTSService : ISpeechAndTTSService
{

    // The speech recognizer used throughout this sample.
    private SpeechRecognizer? speechRecognizer;

    /// <summary>
    /// the HResult 0x8004503a typically represents the case where a recognizer for a particular language cannot
    /// be found. This may occur if the language is installed, but the speech pack for that language is not.
    /// See Settings -> Time & Language -> Region & Language -> *Language* -> Options -> Speech Language Options.
    /// </summary>
    private static readonly uint HResultRecognizerNotFound = 0x8004503a;

    // Keep track of whether the continuous recognizer is currently running, so it can be cleaned up appropriately.
    private bool isListening = false;

    private readonly ILocalSettingsService _localSettingsService;

    public SpeechAndTTSService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public async Task<IRandomAccessStream?> TextToSpeechAsync(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            try
            {
                using SpeechSynthesizer synthesizer = new();
                // Create a stream from the text. This will be played using a media element.
                var synthesisStream = await synthesizer.SynthesizeTextToStreamAsync(text);

                return synthesisStream;
            }
            catch (System.IO.FileNotFoundException)
            {
                return null;
                // If media player components are unavailable, (eg, using a N SKU of windows), we won't
            }
            catch (Exception)
            {
                return null;
            }
        }
        return null;
    }

    public async Task StartAsync()
    {
        if (isListening == false)
        {
            // The recognizer can only start listening in a continuous fashion if the recognizer is currently idle.
            // This prevents an exception from occurring.
            if (speechRecognizer!.State == SpeechRecognizerState.Idle)
            {
                try
                {
                    // Set timeout settings.
                    speechRecognizer.Timeouts.InitialSilenceTimeout = TimeSpan.FromSeconds(3.0);
                    speechRecognizer.Timeouts.BabbleTimeout = TimeSpan.FromSeconds(2.0);
                    speechRecognizer.Timeouts.EndSilenceTimeout = TimeSpan.FromSeconds(1.2);
                    await speechRecognizer.ContinuousRecognitionSession.StartAsync();

                    //var recognitionOperation = speechRecognizer.RecognizeAsync();
                    //SpeechRecognitionResult speechRecognitionResult = await recognitionOperation;

                    isListening = true;
                }
                catch (Exception ex)
                {

                }
            }
        }
        else
        {
            isListening = false;

            if (speechRecognizer!.State != SpeechRecognizerState.Idle)
            {
                try
                {
                    // Cancelling recognition prevents any currently recognized speech from
                    // generating a ResultGenerated event. StopAsync() will allow the final session to 
                    // complete.
                    await speechRecognizer.ContinuousRecognitionSession.CancelAsync();
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
    public async Task CancelAsync()
    {
        try
        {
            // Cancelling recognition prevents any currently recognized speech from
            // generating a ResultGenerated event. StopAsync() will allow the final session to 
            // complete.
            await speechRecognizer?.ContinuousRecognitionSession.CancelAsync();
        }
        catch (Exception ex)
        {
        }
    }

    public async Task InitializeRecognizerAsync(Language recognizerLanguage)
    {
        await InitializeRecognizer(recognizerLanguage);
    }
    public async Task ReleaseRecognizerAsync()
    {
        if (speechRecognizer != null)
        {
            if (isListening)
            {
                await speechRecognizer.ContinuousRecognitionSession.CancelAsync();

                isListening = false;
            }

            speechRecognizer.ContinuousRecognitionSession.Completed -= ContinuousRecognitionSession_Completed;
            speechRecognizer.ContinuousRecognitionSession.ResultGenerated -= ContinuousRecognitionSession_ResultGenerated;
            speechRecognizer.StateChanged -= SpeechRecognizer_StateChanged;

            speechRecognizer.Dispose();
            speechRecognizer = null;
        }
    }



    /// <summary>
    /// Initialize Speech Recognizer and compile constraints.
    /// </summary>
    /// <param name="recognizerLanguage">Language to use for the speech recognizer</param>
    /// <returns>Awaitable task.</returns>
    private async Task InitializeRecognizer(Language recognizerLanguage)
    {
        if (speechRecognizer != null)
        {
            // cleanup prior to re-initializing this scenario.
            speechRecognizer.StateChanged -= SpeechRecognizer_StateChanged;
            speechRecognizer.ContinuousRecognitionSession.Completed -= ContinuousRecognitionSession_Completed;
            speechRecognizer.ContinuousRecognitionSession.ResultGenerated -= ContinuousRecognitionSession_ResultGenerated;

            speechRecognizer.Dispose();
            speechRecognizer = null;
        }

        try
        {
            speechRecognizer = new SpeechRecognizer(recognizerLanguage);

            // Provide feedback to the user about the state of the recognizer. This can be used to provide visual feedback in the form
            // of an audio indicator to help the user understand whether they're being heard.
            speechRecognizer.StateChanged += SpeechRecognizer_StateChanged;
            // Build a command-list grammar. Commands should ideally be drawn from a resource file for localization, and 
            // be grouped into tags for alternate forms of the same command.
            //var bili = new SpeechRecognitionListConstraint(
            //        new List<string>()
            //        {
            //            "打开B站"
            //        }, "Bili");
            //bili.Probability = SpeechRecognitionConstraintProbability.Max;
            //speechRecognizer.Constraints.Add(bili);

            var webSearchGrammar = new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.WebSearch, "webSearch", "sound");
            //webSearchGrammar.Probability = SpeechRecognitionConstraintProbability.Min;
            speechRecognizer.Constraints.Add(webSearchGrammar);
            SpeechRecognitionCompilationResult result = await speechRecognizer.CompileConstraintsAsync();

            if (result.Status != SpeechRecognitionResultStatus.Success)
            {
                // Disable the recognition buttons.
            }
            else
            {
                // Handle continuous recognition events. Completed fires when various error states occur. ResultGenerated fires when
                // some recognized phrases occur, or the garbage rule is hit.
                speechRecognizer.ContinuousRecognitionSession.Completed += ContinuousRecognitionSession_Completed;
                speechRecognizer.ContinuousRecognitionSession.ResultGenerated += ContinuousRecognitionSession_ResultGenerated;
            }
        }
        catch (Exception ex)
        {
            if ((uint)ex.HResult == HResultRecognizerNotFound)
            {

            }
            else
            {

            }
        }

    }

    /// <summary>
    /// Handle events fired when error conditions occur, such as the microphone becoming unavailable, or if
    /// some transient issues occur.
    /// </summary>
    /// <param name="sender">The continuous recognition session</param>
    /// <param name="args">The state of the recognizer</param>
    private void ContinuousRecognitionSession_Completed(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionCompletedEventArgs args)
    {
        if (args.Status == SpeechRecognitionResultStatus.Success)
        {
            isListening = false;
        }
    }

    /// <summary>
    /// Handle events fired when a result is generated. This may include a garbage rule that fires when general room noise
    /// or side-talk is captured (this will have a confidence of Rejected typically, but may occasionally match a rule with
    /// low confidence).
    /// </summary>
    /// <param name="sender">The Recognition session that generated this result</param>
    /// <param name="args">Details about the recognized speech</param>
    private async void ContinuousRecognitionSession_ResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
    {
        // The garbage rule will not have a tag associated with it, the other rules will return a string matching the tag provided
        // when generating the grammar.
        var tag = "unknown";

        if (args.Result.Constraint != null && isListening)
        {
            tag = args.Result.Constraint.Tag;

            App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                ToastHelper.SendToast(tag, TimeSpan.FromSeconds(3));
            });


            Debug.WriteLine($"识别内容---{tag}");
        }

        // Developers may decide to use per-phrase confidence levels in order to tune the behavior of their 
        // grammar based on testing.
        if (args.Result.Confidence == SpeechRecognitionConfidence.Medium ||
            args.Result.Confidence == SpeechRecognitionConfidence.High)
        {
            var result = string.Format("Heard: '{0}', (Tag: '{1}', Confidence: {2})", args.Result.Text, tag, args.Result.Confidence.ToString());


            App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                ToastHelper.SendToast(result, TimeSpan.FromSeconds(3));
            });


            if (args.Result.Text.ToUpper() == "打开B站")
            {
                await Launcher.LaunchUriAsync(new Uri(@"https://www.bilibili.com/"));
            }
            else if (args.Result.Text.ToUpper() == "撒个娇")
            {
                ElectronBotHelper.Instance.ToPlayEmojisRandom();
            }
            else if (args.Result.Text.ToUpper().StartsWith("启动"))
            {
               await  LaunchAppAsync(args.Result.Text.ToUpper().Replace("启动", ""));
            }
            else if (args.Result.Text.ToUpper().StartsWith("打开"))
            {
                await LaunchAppAsync(args.Result.Text.ToUpper().Replace("打开", ""));
            }
            else if (args.Result.Text.ToUpper().StartsWith("open"))
            {
                await LaunchAppAsync(args.Result.Text.ToUpper().Replace("open ", ""));
            }
            else
            {
                try
                {
                    //var chatGPTClient = App.GetService<IChatGPTService>();

                    //var resultText = await chatGPTClient.AskQuestionResultAsync(args.Result.Text);

                    //await ElectronBotHelper.Instance.MediaPlayerPlaySoundByTTSAsync(resultText);

                    var chatBotClientFactory = App.GetService<IChatbotClientFactory>();

                    var chatBotClientName = (await App.GetService<ILocalSettingsService>()
                         .ReadSettingAsync<ComboxItemModel>(Constants.DefaultChatBotNameKey))?.DataKey;

                    if (string.IsNullOrEmpty(chatBotClientName))
                    {
                        throw new Exception("未配置语音提供程序机密数据");
                    }

                    var chatBotClient = chatBotClientFactory.CreateChatbotClient(chatBotClientName);

                    var resultText = await chatBotClient.AskQuestionResultAsync(args.Result.Text);

                    //isListening = false;
                    await ReleaseRecognizerAsync();

                    await ElectronBotHelper.Instance.MediaPlayerPlaySoundByTtsAsync(resultText, false);      
                }
                catch (Exception ex)
                {
                    App.MainWindow.DispatcherQueue.TryEnqueue(() =>
                    {
                        ToastHelper.SendToast(ex.Message, TimeSpan.FromSeconds(3));
                    });

                }
            }
        }
        else
        {
        }
    }

    /// <summary>
    /// Provide feedback to the user based on whether the recognizer is receiving their voice input.
    /// </summary>
    /// <param name="sender">The recognizer that is currently running.</param>
    /// <param name="args">The current state of the recognizer.</param>
    private void SpeechRecognizer_StateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            ToastHelper.SendToast(args.State.ToString(), TimeSpan.FromSeconds(3));
        });

        if (args.State == SpeechRecognizerState.Capturing)
        {
            isListening = true;
        }

        if (args.State == SpeechRecognizerState.SoundEnded)
        {
            isListening = false;
        }
    }

    private async Task LaunchAppAsync(string appFullName)
    {
        var launchAppConfigs = (await _localSettingsService.ReadSettingAsync<List<LaunchAppConfig>>(Constants.LaunchAppConfigKey)) ?? new List<LaunchAppConfig>();

        var appConfig = launchAppConfigs.FirstOrDefault(l=>l.VoiceText == appFullName);

        var appName = appFullName;

        if (appConfig != null)
        {
            appName = appConfig.AppNameText;

            if (!appConfig.IsMsix)
            {
                try
                {
                    
                    var appText = $"正在唤醒{appConfig.VoiceText}";
                    App.MainWindow.DispatcherQueue.TryEnqueue(() =>
                    {
                        ToastHelper.SendToast(appText, TimeSpan.FromSeconds(2));
                    });
                    await ElectronBotHelper.Instance.MediaPlayerPlaySoundByTtsAsync(appText, true);
                    await WindowHelper.Instance.StartProcess(appConfig.Win32Path!);
                    return;
                }
                catch (Exception ex)
                {
                    App.MainWindow.DispatcherQueue.TryEnqueue(() =>
                    {
                        ToastHelper.SendToast(ex.Message, TimeSpan.FromSeconds(3));
                    });
                }
            }
        }

        var package = ElectronBotHelper.Instance.AppPackages.Where(x => x.DisplayName.ToUpper().StartsWith(appName.ToUpper())).FirstOrDefault();

        if (ElectronBotHelper.Instance.AppPackages.Where(x => x.DisplayName.ToUpper().StartsWith(appName.ToUpper())).FirstOrDefault() == null)
        {
            package = ElectronBotHelper.Instance.AppPackages.Where(x => x.DisplayName.ToUpper().Contains(appName.ToUpper())).FirstOrDefault();
        }
        if (package != null)
        {
            try
            {
              
                var appText = $"正在唤醒{appName}";
                App.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    ToastHelper.SendToast(appText, TimeSpan.FromSeconds(2));
                });
                await ElectronBotHelper.Instance.MediaPlayerPlaySoundByTtsAsync(appText, true);

                await package.GetAppListEntries()[0].LaunchAsync();
            }
            catch(Exception ex)
            {
                App.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    ToastHelper.SendToast(ex.Message, TimeSpan.FromSeconds(3));
                });
            }
        }
    }
}
