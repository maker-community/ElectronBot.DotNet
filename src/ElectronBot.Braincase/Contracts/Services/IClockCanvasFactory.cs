using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verdure.Braincase.Contracts.Services;
public interface IClockCanvasFactory
{
    IClockCanvas CreateClockCanvas(string canvasName);

    void AddProvider(IClockCanvasProvider provider);
}
