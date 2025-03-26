using CommunityToolkit.Mvvm.ComponentModel;
using Windows.ApplicationModel;
using Windows.System;

namespace Verdure.Braincase.Settings.ViewModels;

public partial class AboutAppViewModel : ObservableRecipient
{
    public AboutAppViewModel()
    {
        VersionDescription = GetVersionDescription();
    }

    [ObservableProperty]
    private string _versionDescription;


    [RelayCommand]
    public async Task FeedbackBtnAsync()
    {
        await FeedbackAsync("gil.zhang.dev@outlook.com", "反馈", "这是一些反馈");
    }

    public async Task FeedbackAsync(string address, string subject, string body)
    {
        if (address == null)
        {
            return;
        }
        var mailto = new Uri($"mailto:{address}?subject={subject}&body={body}");
        await Launcher.LaunchUriAsync(mailto);
    }
    private static string GetVersionDescription()
    {
        var appName = "AppDisplayName".GetLocalized();

        var version = Package.Current.Id.Version;

        return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }
}
