using System.Linq;
using ElectronBot.Braincase.Contracts.Services;
using Verdure.Braincase.Core.Models;
using Verdure.WinUI.Common.Services;
using Windows.Devices.Enumeration;

namespace Verdure.WinUI.Common.Players;
public partial class ElectronBotPlayer
{
    public async Task PlayAudioByTextAsync(string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            try
            {
                var localSettingsService = Ioc.Default.GetRequiredService<ILocalSettingsService>();

                var audioModel = await localSettingsService.ReadSettingAsync<ComboxItemModel>(CommonConstants.DefaultAudioNameKey);

                var audioDevs = await FindAudioDeviceListAsync();

                if (audioModel != null)
                {
                    var audioSelect = audioDevs.FirstOrDefault(c => c.DataValue == audioModel.DataValue) ?? new ComboxItemModel();

                    var selectedDevice = (DeviceInformation)audioSelect.Tag!;

                    if (selectedDevice != null)
                    {
                        _player.AudioDevice = selectedDevice;
                    }
                }

                var speechAndTTSService = Ioc.Default.GetRequiredService<ISpeechAndTTSService>();

                var stream = await speechAndTTSService.TextToSpeechAsync(text);

                _player.SetStreamSource(stream);
                _player.Play();
            }
            catch (Exception)
            {
            }
        }
    }
    public Task PlayAudioByPathAsync(string path) => throw new NotImplementedException();
    public Task PlayAudioAsync(Stream stream) => throw new NotImplementedException();
}
