using System;
using System.Collections.Generic;
using System.Text;

namespace Verdure.Braincase.Core.Models;
public class ComboxItemModel
{
    public object? Tag
    {
        get; set;
    }
    public string? DataKey
    {
        get; set;
    }
    public string? DataValue
    {
        get; set;
    }
}
