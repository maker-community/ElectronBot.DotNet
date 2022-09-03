namespace ElectronBot.BraincasePreview.Contracts.Services;
public interface IActionExpressionProviderFactory
{
    IActionExpressionProvider CreateActionExpressionProvider(string actionName);
}
