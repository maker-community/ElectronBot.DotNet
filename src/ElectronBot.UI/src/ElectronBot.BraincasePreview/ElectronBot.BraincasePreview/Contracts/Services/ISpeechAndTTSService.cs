using System;
using System.Collections.Generic;
using System.Text;
using Windows.Globalization;
using Windows.Media.SpeechRecognition;

namespace ElectronBot.BraincasePreview.Contracts.Services;
public interface ISpeechAndTTSService
{
    Task StartAsync();
    Task CancelAsync();
    Task InitializeRecognizerAsync(Language recognizerLanguage);
    Task ReleaseRecognizerAsync();
}
