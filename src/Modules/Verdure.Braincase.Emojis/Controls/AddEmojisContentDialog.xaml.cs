using Verdure.Braincase.ViewModels;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace Verdure.Braincase.Controls;

public sealed partial class AddEmojisContentDialog : ContentDialog
{
    public AddEmojisDialogViewModel ViewModel
    {
        get;

    }
    public AddEmojisContentDialog()
    {
        ViewModel = Ioc.Default.GetRequiredService<AddEmojisDialogViewModel>();

        DataContext = ViewModel;

        InitializeComponent();
    }

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {

    }

    private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {

    }
}
