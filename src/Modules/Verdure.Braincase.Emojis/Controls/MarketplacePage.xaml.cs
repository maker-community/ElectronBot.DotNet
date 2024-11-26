// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Verdure.Braincase.Emojis.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Controls;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MarketplacePage : Page
{
    public MarketplaceViewModel ViewModel
    {
        get;
    }

    public MarketplacePage()
    {
        ViewModel = Ioc.Default.GetRequiredService<MarketplaceViewModel>();
        this.InitializeComponent();
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.Loaded();
    }
}
