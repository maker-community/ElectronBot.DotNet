using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Verdure.Braincase.WinUI.Common.Models;
public class LottieFrameEventArgs
{
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
