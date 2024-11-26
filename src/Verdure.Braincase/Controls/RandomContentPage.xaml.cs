// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Verdure.Braincase;
using Verdure.Braincase.ViewModels;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RandomContentPage : Page
    {
        public RandomContentViewModel ViewModel
        {
            get;
        }

        public RandomContentPage()
        {
            ViewModel = Ioc.Default.GetRequiredService<RandomContentViewModel>();
            this.InitializeComponent();
        }
    }
}
