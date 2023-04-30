using System;
using System.Collections.Generic;
using ElectronBot.Braincase.Contracts.Services;

namespace ElectronBot.Braincase.Services;
public class ClockCanvasFactory : IClockCanvasFactory
{
    private readonly Dictionary<string, IClockCanvas> _canvasList = new Dictionary<string, IClockCanvas>(StringComparer.Ordinal);
    public void AddProvider(IClockCanvasProvider provider) => throw new NotImplementedException();

    public ClockCanvasFactory(IEnumerable<IClockCanvasProvider> clockCanvasList)
    {
        //foreach (IClockCanvasProvider provider in clockCanvasList)
        //{
        //    AddProviderRegistration(provider, dispose: false);
        //}
    }
    public IClockCanvas CreateClockCanvas(string canvasName)
    {
        if (_canvasList.TryGetValue(canvasName, out IClockCanvas canvas))
        {
            return canvas;
        }
        else
        {
            return null;
        }

    }
}
