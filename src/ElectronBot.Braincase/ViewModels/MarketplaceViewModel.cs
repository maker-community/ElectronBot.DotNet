using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Contracts.Services;
using ElectronBot.Braincase.Helpers;
using Models;

namespace ElectronBot.Braincase.ViewModels;

public partial class MarketplaceViewModel : ObservableRecipient
{
    private ObservableCollection<EmojisItemDto> _emojisList = new();

    private readonly IEmojiseShopService _emojiseShopService;

    public MarketplaceViewModel(
        IEmojiseShopService emojiseShopService)
    {
        _emojiseShopService = emojiseShopService;
    }

    [RelayCommand]
    public async Task Loaded()
    {
        try
        {
            var list = await _emojiseShopService.GetEmojisListAsync(new EmojisItemQuery { PageIndex = 0, PageSize = 100 });
            EmojisList = new ObservableCollection<EmojisItemDto>(list);
        }
        catch (Exception)
        {
            ToastHelper.SendToast("表情商城加载失败", TimeSpan.FromSeconds(3));
        }
    }

    [RelayCommand]
    public async void DownLoadEmojis(object? obj)
    {
        if (obj is EmojisItemDto emojis)
        {
            IsActive = true;

            var ret = await _emojiseShopService.DownloadEmojisAsync(emojis.VideoFileId);

            if (ret is not null)
            {
                ToastHelper.SendToast("下载表情成功", TimeSpan.FromSeconds(3));
            }
            //else
            //{
            //    ToastHelper.SendToast("表情下载失败", TimeSpan.FromSeconds(3));
            //}

            IsActive = false;
        }

    }

    [ObservableProperty]
    private bool isActive = false;

    public ObservableCollection<EmojisItemDto> EmojisList
    {
        get => _emojisList;
        set => SetProperty(ref _emojisList, value);
    }
}
