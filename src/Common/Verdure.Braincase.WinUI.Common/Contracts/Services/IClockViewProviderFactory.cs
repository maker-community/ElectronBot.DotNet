namespace Verdure.Braincase.WinUI.Common.Contracts.Services;
public interface IClockViewProviderFactory
{
    IClockViewProvider CreateClockViewProvider(string viewName);
}
