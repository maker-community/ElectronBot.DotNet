using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verdure.Braincase.Contracts.Services;
public interface IClockCanvasProviderFactory
{
    IClockCanvasProvider CreateClockCanvasProvider(string canvasName);
}
