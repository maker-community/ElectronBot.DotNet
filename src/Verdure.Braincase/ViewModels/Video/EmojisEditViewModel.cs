using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.Collections;
using Controls;
using Verdure.Braincase.Contracts.Services;
using Verdure.Braincase.Helpers;
using Verdure.Braincase.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Models;
using Verdure.Braincase.WinUI.Common;
using Verdure.Braincase.WinUI.Common.Helpers;
using Verdure.Braincase.WinUI.Common.Models;
using Verdure.Braincase.WinUI.Common.ViewDataSource;
using Windows.ApplicationModel;
using Windows.Storage;
using Verdure.Braincase.Core.Contracts.Services.EmojisFile;

namespace Verdure.Braincase.ViewModels;

public partial class EmojisEditViewModel : ObservableRecipient
{
    private readonly ElementTheme _elementTheme;

    private readonly ILocalSettingsService _localSettingsService;

    private readonly IEmojisFileService _emojisFileService;

    private readonly IElectronBotPlayer _electronBotPlayer;


    private readonly IntPtr _hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
    public EmojisEditViewModel(ILocalSettingsService localSettingsService,
        IEmojisFileService emojisFileService,
        IThemeSelectorService themeSelectorService,
        IElectronBotPlayer electronBotPlayer)
    {
        _localSettingsService = localSettingsService;
        _emojisFileService = emojisFileService;
        _elementTheme = themeSelectorService.Theme;
        _electronBotPlayer = electronBotPlayer;
    }

    private async void MarketplaceDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        await OnLoadedAsync();
    }


    private void EmojisInfoContentDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        if (sender.Content is EmojisInfoPage page)
        {
            if (page.DataContext is EmojisInfoDialogViewModel viewModel)
            {
                if (viewModel is not null)
                {
                    var emotion = viewModel.EmoticonAction;

                    if (emotion is not null)
                    {
                        var act = Actions.Where(a => a.NameId == emotion.NameId).FirstOrDefault();
                        if (act is not null)
                        {
                            act.HasAction = emotion.HasAction;
                        }
                    }
                }
            }
        }
    }


    private async void AddEmojisContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (sender.Content is AddEmojisPage page)
        {
            if (page.DataContext is AddEmojisDialogViewModel viewModel)
            {
                if (viewModel is not null)
                {
                    if (string.IsNullOrWhiteSpace(viewModel.EmojisNameId))
                    {
                        ToastHelper.SendToast("SetEmojisNameId".GetLocalized(), TimeSpan.FromSeconds(3));
                        args.Cancel = true;
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(viewModel.EmojisName))
                    {
                        ToastHelper.SendToast("SetEmojisName".GetLocalized(), TimeSpan.FromSeconds(3));
                        args.Cancel = true;
                        return;
                    }


                    var list = (await _localSettingsService
                        .ReadSettingAsync<List<EmoticonAction>>(Constants.EmojisActionListKey)) ?? new List<EmoticonAction>();

                    if (list.Where(e => e.NameId == viewModel.EmojisNameId).Any() || Constants.EMOJI_ACTION_LIST.Where(e => e.NameId == viewModel.EmojisNameId).Any())
                    {
                        ToastHelper.SendToast("EmojisNameIdAlreadyExists".GetLocalized(), TimeSpan.FromSeconds(3));
                        args.Cancel = true;
                        return;
                    }
                    viewModel.SaveEmojis();
                }
            }
        }
    }

    private void AddEmojisContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
    {
        if (sender.Content is AddEmojisPage page)
        {
            if (page.DataContext is AddEmojisDialogViewModel viewModel)
            {
                if (viewModel is not null)
                {
                    viewModel.SaveEmojis();
                }
            }
        }
    }

    private void AddEmojisContentDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        if (sender.Content is AddEmojisPage page)
        {
            if (page.DataContext is AddEmojisDialogViewModel viewModel)
            {
                if (viewModel is not null)
                {
                    var emotion = viewModel.EmoticonAction;

                    if (emotion is not null)
                    {
                        Actions.Add(emotion);
                    }
                }
            }
        }
    }

    [RelayCommand]
    public Task OnLoadedAsync()
    {
        Emojis.Clear();
        // IncrementalLoadingCollection can be bound to a GridView or a ListView. In this case it is a ListView called PeopleListView.
        Emojis = new IncrementalLoadingCollection<EmojisSource, EmoticonActionUIModel>(Ioc.Default.GetRequiredService<EmojisSource>());
        return Task.CompletedTask;
    }
}
