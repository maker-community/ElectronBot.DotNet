using System.Linq;
using CommunityToolkit.Mvvm.DependencyInjection;
using ElectronBot.Braincase.Contracts.Services;
using Verdure.Braincase.Core.Models;
using Verdure.WinUI.Common.Models;
using Windows.Devices.Enumeration;
using Windows.Media.Devices;

namespace Verdure.WinUI.Common.Players;
public partial class ElectronBotPlayer
{
    public Task PlayVideoByNameIdAsync(string nameId) => throw new NotImplementedException();
    public async Task PlayVideoByPathAsync(string path, List<ElectronBotAction>? actions = null)
    {
        if (!string.IsNullOrWhiteSpace(path))
        {
            try
            {
                _actions = actions;
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
                _player.SetUriSource(new Uri(path));
                _player.Play();
            }
            catch (Exception)
            {

            }
        }
    }

    /// <summary>
    /// 获取音频设备列表
    /// </summary>
    /// <returns></returns>
    private static async Task<List<ComboxItemModel>> FindAudioDeviceListAsync()
    {
        List<ComboxItemModel> ret = new();

        var audioSelector = MediaDevice.GetAudioRenderSelector();

        var allVideoDevices = await DeviceInformation.FindAllAsync(audioSelector);

        if (allVideoDevices != null && allVideoDevices.Count > 0)
        {
            var devList = allVideoDevices.ToList();

            foreach (var dev in devList)
            {
                ComboxItemModel combox = new()
                {
                    Tag = dev,
                    DataKey = devList.IndexOf(dev).ToString(),
                    DataValue = dev.Name
                };

                ret.Add(combox);
            }
        }

        return ret;
    }
}
