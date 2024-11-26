namespace Verdure.Braincase.WinUI.Common.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}
