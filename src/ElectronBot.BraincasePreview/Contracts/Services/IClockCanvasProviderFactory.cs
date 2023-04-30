using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronBot.Braincase.Contracts.Services;
public interface IClockCanvasProviderFactory
{
    IClockCanvasProvider CreateClockCanvasProvider(string canvasName);
}
