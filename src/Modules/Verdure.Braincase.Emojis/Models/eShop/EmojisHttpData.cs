using Models;

namespace Verdure.Braincase.Emojis.Models.eShop;


public class EmojisHttpData
{
    public int PageIndex
    {
        get; set;
    }
    public int PageSize
    {
        get; set;
    }
    public int Count
    {
        get; set;
    }
    public List<EmojisItemDto> Items
    {
        get; set;
    } = new();
}
