namespace ElectronBot.BraincasePreview.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}
