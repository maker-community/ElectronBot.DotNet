using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.BraincasePreview.Core.Models;

namespace ElectronBot.BraincasePreview.ViewModels;

public class EmojisEditViewModel : ObservableRecipient
{
    private ObservableCollection<EmoticonAction> _actions = new();

    private ICommand _loadedCommand;
    public ICommand LoadedCommand => _loadedCommand ??= new RelayCommand(OnLoaded);
    public EmojisEditViewModel()
    {
    }

    public ObservableCollection<EmoticonAction> Actions
    {
        get => _actions;
        set => SetProperty(ref _actions, value);
    }

    private void OnLoaded()
    {
        var emoticonActions = new List<EmoticonAction>()
        {
            new EmoticonAction
            {
                Name = "开心1",
                Avatar = "ms-appx:///Assets/cat_with_wry_smile_3d.png",
                Desc = "这是开心的表情",
                NameKey ="happy1"
            },
            new EmoticonAction
            {
                Name = "开心2",
                Avatar = "ms-appx:///Assets/cat_with_wry_smile_3d.png",
                Desc = "这是开心的表情",
                NameKey ="happy2"
            },
            new EmoticonAction
            {
                Name = "开心3",
                Avatar = "ms-appx:///Assets/cat_with_wry_smile_3d.png",
                Desc = "这是开心的表情",
                NameKey ="happy3"
            },
            new EmoticonAction
            {
                Name = "开心4",
                Avatar = "ms-appx:///Assets/cat_with_wry_smile_3d.png",
                Desc = "这是开心的表情",
                NameKey ="happy4"
            },

        };
        Actions = new ObservableCollection<EmoticonAction>(emoticonActions);
    }
}
