using System;
using System.Collections.Generic;
using ElectronBot.Braincase.Contracts.Services;

namespace ElectronBot.Braincase.Services;
public class Hw75DynamicViewProviderFactory : IHw75DynamicViewProviderFactory
{
    private readonly Dictionary<string, IHw75DynamicViewProvider> _providers = new(StringComparer.Ordinal);
    public Hw75DynamicViewProviderFactory(IEnumerable<IHw75DynamicViewProvider> providers)
    {
        foreach (var provider in providers)
        {
            _providers.Add(provider.Name, provider);
        }
    }
    public IHw75DynamicViewProvider CreateHw75DynamicViewProvider(string viewName)
    {
        return _providers.ContainsKey(viewName) ? _providers[viewName] : null;
    }
}
