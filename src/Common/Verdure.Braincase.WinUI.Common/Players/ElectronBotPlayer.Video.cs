using System.Linq;
using System.Text.Json;
using Verdure.Braincase.Core.Contracts.Services.EmojisFile;
using Verdure.Braincase.Core.Models;
using Verdure.Braincase.WinUI.Common.Models;
using Windows.Devices.Enumeration;
using Windows.Media.Devices;
using Windows.Storage.Streams;

namespace Verdure.Braincase.WinUI.Common.Players;
public partial class ElectronBotPlayer
{
    public async Task PlayVideoByNameIdAsync(string nameId)
    {
        if (!string.IsNullOrWhiteSpace(nameId))
        {
            try
            {
                var emojisFileService = Ioc.Default.GetRequiredService<IEmojisFileService>();

                var emotion = await emojisFileService.GetEmojisFileWithVideoStreamAsync(nameId);


                _actions = JsonSerializer.Deserialize<List<ElectronBotAction>>(emotion.EmojisActionJson);

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
                IRandomAccessStream randomAccessStream = emotion.EmojisVideo.AsRandomAccessStream();

                _player.SetStreamSource(randomAccessStream);
                _player.Play();
            }
            catch (Exception)
            {

            }
        }
    }
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
    public static async Task<List<ComboxItemModel>> FindAudioDeviceListAsync()
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

    /// <summary>
    /// 获取相机设备列表
    /// </summary>
    /// <returns></returns>
    public static async Task<List<ComboxItemModel>> FindCameraDeviceListAsync()
    {
        List<ComboxItemModel> ret = new();
        // Get available devices for capturing pictures
        var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

        if (allVideoDevices != null && allVideoDevices.Count > 0)
        {
            var devList = allVideoDevices.ToList();

            foreach (var dev in devList)
            {
                ComboxItemModel combox = new()
                {
                    DataKey = devList.IndexOf(dev).ToString(),
                    DataValue = dev.Name,
                    Tag = dev
                };

                ret.Add(combox);
            }
        }

        return ret;
    }
}
