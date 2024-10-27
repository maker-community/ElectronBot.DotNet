using LiteDB;

namespace Verdure.Braincase.DataStorage;

public abstract class LiteDBBase
{
    private string? id;

    public string Id
    {
        get
        {
            if (string.IsNullOrEmpty(id))
            {
                id = GenerateId();
            }
            return id;
        }
        set => id = value;
    }

    private string GenerateId()
    {
        // Generate and return the id here
        // You can use any logic to generate the id
        // For example, you can use Guid.NewGuid().ToString() to generate a unique id
        return Guid.NewGuid().ToString();
    }
}


