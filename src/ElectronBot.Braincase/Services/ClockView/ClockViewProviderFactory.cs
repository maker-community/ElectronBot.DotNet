using System;
using System.Collections.Generic;
using ElectronBot.Braincase.Contracts.Services;

namespace ElectronBot.Braincase.Services;
public class ClockViewProviderFactory : IClockViewProviderFactory
{
    private readonly Dictionary<string, IClockViewProvider> _providers = new(StringComparer.Ordinal);
    public ClockViewProviderFactory(IEnumerable<IClockViewProvider> providers)
    {
        foreach (var provider in providers)
        {
            _providers.Add(provider.Name, provider);
        }
    }
    public IClockViewProvider CreateClockViewProvider(string viewName)
    {
        return _providers.ContainsKey(viewName) ? _providers[viewName] : null;
    }
}
