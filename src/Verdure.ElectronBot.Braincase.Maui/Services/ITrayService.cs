namespace Verdure.ElectronBot.Braincase.Maui.Services;

public interface ITrayService
{
    void Initialize();

    Action ClickHandler { get; set; }
}
