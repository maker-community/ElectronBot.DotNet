using Verdure.Braincase.Contracts.Services;

namespace Verdure.Braincase.Services;
public class DefaultClockCanvasProvider : IClockCanvasProvider
{
    private readonly string _name = "DefautCanvas";
    public string Name => _name;

    public IClockCanvas CreateIClockCanvas(string canvasName)
    {
        return new DefaultClockCanvas(canvasName);
    }
}
