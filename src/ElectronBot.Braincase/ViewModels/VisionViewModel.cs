using CommunityToolkit.Mvvm.ComponentModel;
using ElectronBot.Braincase.Contracts.ViewModels;
using Services.ElectronBot;

namespace ElectronBot.Braincase.ViewModels;

public class VisionViewModel : ObservableRecipient, INavigationAware
{
    public VisionViewModel()
    {
        CurrentEmojis._emojis = new EmojiCollection();
    }

    public async void OnNavigatedFrom()
    {
        await VisionService.Current.StopAsync();
    }
    public async void OnNavigatedTo(object parameter)
    {
        await VisionService.Current.StartAsync();
    }
}
