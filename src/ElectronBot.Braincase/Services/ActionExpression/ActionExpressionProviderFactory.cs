using System;
using System.Collections.Generic;
using ElectronBot.Braincase.Contracts.Services;

namespace ElectronBot.Braincase.Services;
public class ActionExpressionProviderFactory : IActionExpressionProviderFactory
{
    private readonly Dictionary<string, IActionExpressionProvider> _providers = new(StringComparer.Ordinal);
    public ActionExpressionProviderFactory(IEnumerable<IActionExpressionProvider> providers)
    {
        foreach (var provider in providers)
        {
            _providers.Add(provider.Name, provider);
        }
    }
    public IActionExpressionProvider CreateActionExpressionProvider(string actionName)
    {
        return _providers.ContainsKey(actionName) ? _providers[actionName] : null;
    }
}
