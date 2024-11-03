using Windows.Storage;

namespace Verdure.Braincase.WinUI.Common.Models;

public class ImageCropperConfig
{
    public StorageFile ImageFile
    {
        get; set;
    }
    public double AspectRatio { get; set; } = -1;
    public bool CircularCrop
    {
        get; set;
    }
}
