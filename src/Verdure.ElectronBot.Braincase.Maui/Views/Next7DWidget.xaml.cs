using Verdure.ElectronBot.Braincase.Maui.ViewModels;

namespace Verdure.ElectronBot.Braincase.Maui.Views;

public partial class Next7DWidget
{
    public Next7DWidget()
    {
        InitializeComponent();

        BindingContext = new HomeViewModel();
    }
}
