using System.ComponentModel.DataAnnotations;

namespace Verdure.ElectronBot.Core.Models;
public class CatalogItemRequest
{
    [Required]
    public string Name { get; private set; } = string.Empty;
    [Required]
    public string PictureFileName { get; private set; } = string.Empty;
    [Required]
    public string PictureFileId { get; private set; } = string.Empty;
    [Required]
    public string VideoFileId { get; private set; } = string.Empty;

    public int CatalogTypeId
    {
        get; private set;
    }

    public int CatalogBrandId
    {
        get; private set;
    }
}
