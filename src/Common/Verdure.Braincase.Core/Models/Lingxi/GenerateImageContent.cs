namespace Verdure.ElectronBot.Core.Models;
public class GenerateImageContent
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string ImageData
    {
        get; set;
    } = string.Empty;
}
