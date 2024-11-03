// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Verdure.Braincase.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Controls;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class UploadEmojisPage : Page
{

    public UploadEmojisDialogViewModel ViewModel
    {
        get;

    }
    public UploadEmojisPage()
    {
        ViewModel = Ioc.Default.GetRequiredService<UploadEmojisDialogViewModel>();

        DataContext = ViewModel;

        InitializeComponent();
    }

    public EmoticonAction EmoticonAction
    {
        get; set;
    }


    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {

    }

    private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {

    }
}
