using Verdure.ElectronBot.Braincase.Maui.ViewModels;

namespace Verdure.ElectronBot.Braincase.Maui.Views;

public partial class Next24HrWidget
{
    public Next24HrWidget()
    {
        InitializeComponent();

        BindingContext = new HomeViewModel();
    }
}
