using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;

namespace ElectronBot.Braincase.ViewModels;

public class UserViewModel : ObservableObject
{
    private string _name;
    private string _userPrincipalName;
    private BitmapImage _photo;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string UserPrincipalName
    {
        get => _userPrincipalName;
        set => SetProperty(ref _userPrincipalName, value);
    }

    public BitmapImage Photo
    {
        get => _photo;
        set => SetProperty(ref _photo, value);
    }

    public UserViewModel()
    {
    }
}
