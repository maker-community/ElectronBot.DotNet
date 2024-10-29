using ElectronBot.Braincase.Models;
using Verdure.Braincase.DataStorage.Collections;

namespace Verdure.Braincase.DataStorage.Mappers;

public static class EmojisMappers
{
    public static EmojisDocument ToDoc(this EmoticonAction model)
    {
        return new EmojisDocument
        {

        };
    }

    public static EmoticonAction ToModel(this EmojisDocument model)
    {
        return new EmoticonAction
        {
        };
    }
}
