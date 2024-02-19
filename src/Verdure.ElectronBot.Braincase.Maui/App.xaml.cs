using System.Diagnostics;

namespace Verdure.ElectronBot.Braincase.Maui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        if (DeviceInfo.Idiom == DeviceIdiom.Phone)
            Shell.Current.CurrentItem = PhoneTabs;
    }
    
    async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
    {
        try { 
            await Shell.Current.GoToAsync($"///settings");
        }catch (Exception ex) {
            Debug.WriteLine($"err: {ex.Message}");
        }
    }
}