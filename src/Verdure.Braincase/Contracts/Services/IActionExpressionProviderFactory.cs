namespace Verdure.Braincase.Contracts.Services;
public interface IActionExpressionProviderFactory
{
    IActionExpressionProvider CreateActionExpressionProvider(string actionName);
}
