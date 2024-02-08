using System.Diagnostics;

namespace Verdure.ElectronBot.Braincase.Maui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
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