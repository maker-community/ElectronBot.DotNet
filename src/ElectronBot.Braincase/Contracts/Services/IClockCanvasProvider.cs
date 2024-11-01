using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Media.Imaging;

namespace Verdure.Braincase.Contracts.Services;
public interface IClockCanvasProvider
{
    public string Name
    {
        get;
    }
    IClockCanvas CreateIClockCanvas(string canvasName);
}
