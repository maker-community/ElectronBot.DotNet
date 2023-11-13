// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using ElectronBot.Braincase;
using ElectronBot.Braincase.ViewModels;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LaunchAppPage : Page
    {
        public LaunchAppViewModel ViewModel
        {
            get;
        }

        public LaunchAppPage()
        {
            ViewModel = App.GetService<LaunchAppViewModel>();
            DataContext = ViewModel;
            this.InitializeComponent();
        }
    }
}
