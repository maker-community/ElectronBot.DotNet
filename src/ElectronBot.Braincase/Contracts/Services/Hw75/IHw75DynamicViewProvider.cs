using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;

namespace ElectronBot.Braincase.Contracts.Services;
public interface IHw75DynamicViewProvider
{
    public string Name
    {
        get;
    }
    UIElement CreateHw75DynamickView(string viewName);
}
