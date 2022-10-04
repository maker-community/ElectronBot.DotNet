using ElectronBot.BraincasePreview.Contracts.Services;

namespace ElectronBot.BraincasePreview.Services;
public class DefaultClockCanvasProvider : IClockCanvasProvider
{
    private readonly string _name = "DefautCanvas";
    public string Name => _name;

    public IClockCanvas CreateIClockCanvas(string canvasName)
    {
        return new DefaultClockCanvas(canvasName);
    }
}
