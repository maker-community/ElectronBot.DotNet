using Verdure.Braincase.Core.Models;

namespace Verdure.Braincase.WinUI.Common.Models;
public class LottieFrameEventArgs
{
    public EmoticonActionFrame ActionFrameData
    {
        get; set;
    } = new();
    public byte[] FrameData
    {
        get; set;
    } = [];

    public int Width
    {
        get; set;
    }

    public int Height
    {
        get; set;
    }
}
