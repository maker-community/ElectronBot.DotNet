using ElectronBot.Braincase.Contracts.Services;

namespace ElectronBot.Braincase.Services;
public class DefaultClockCanvasProvider : IClockCanvasProvider
{
    private readonly string _name = "DefautCanvas";
    public string Name => _name;

    public IClockCanvas CreateIClockCanvas(string canvasName)
    {
        return new DefaultClockCanvas(canvasName);
    }
}
