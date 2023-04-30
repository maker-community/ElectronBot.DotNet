using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Core.Models;
using ElectronBot.Braincase.Helpers;

namespace ElectronBot.Braincase.ViewModels;

public partial class RandomContentViewModel : ObservableRecipient
{
    private readonly ILocalSettingsService _localSettingsService;
    public RandomContentViewModel(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }


    [ObservableProperty]
    private string randomContent = string.Empty;

    [ObservableProperty]
    private ObservableCollection<RandomContent> randomContentList = new();

    [RelayCommand]
    public async void DelRandomContent(object? obj)
    {
        if (obj == null)
        {
            ToastHelper.SendToast("请选中一个内容", TimeSpan.FromSeconds(3));
            return;
        }
        if (obj is RandomContent randomContent)
        {
            try
            {

                RandomContentList.Remove(randomContent);

                await _localSettingsService.SaveSettingAsync(Constants.RandomContentListKey, RandomContentList.ToList());
            }
            catch (Exception)
            {

            }
        }
    }
    [RelayCommand]
    public async void SaveContent()
    {
        if (RandomContentList.Where(e => e.Content == randomContent).Any())
        {
            ToastHelper.SendToast("ContentAlreadyExists".GetLocalized(), TimeSpan.FromSeconds(3));

            return;
        }

        if (!string.IsNullOrWhiteSpace(RandomContent))
        {
            var content = new RandomContent
            {
                Content = RandomContent
            };

            RandomContentList.Add(content);

            await _localSettingsService.SaveSettingAsync(Constants.RandomContentListKey, RandomContentList.ToList());
        }
    }

    [RelayCommand]
    public async void Loaded()
    {
        var list = (await _localSettingsService.ReadSettingAsync<List<RandomContent>>(Constants.RandomContentListKey)) ?? new List<RandomContent>();

        await Task.Delay(TimeSpan.FromSeconds(1));

        RandomContentList = new(list);
    }
}
