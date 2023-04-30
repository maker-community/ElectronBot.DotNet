using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronBot.Braincase.Contracts.Services;
public interface IClockCanvasFactory
{
    IClockCanvas CreateClockCanvas(string canvasName);

    void AddProvider(IClockCanvasProvider provider);
}
