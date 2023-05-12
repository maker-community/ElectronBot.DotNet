using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.CompactOverlay;
using ElectronBot.Braincase;
using Microsoft.UI.Windowing;

namespace ViewModels;
public partial class TodoCompactOverlayViewModel : ObservableRecipient
{
    [RelayCommand]
    public void CompactOverlay()
    {
        //WindowEx compactOverlay = new CompactOverlayWindow();

        //compactOverlay.Content = new DefaultCompactOverlayPage();

        //var appWindow = compactOverlay.AppWindow;

        //appWindow.SetPresenter(AppWindowPresenterKind.CompactOverlay);

        //appWindow.Destroy();

        App.MainWindow.Show();
    }
}
