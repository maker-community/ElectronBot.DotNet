using System;
using System.Collections.Generic;
using System.Text;

namespace Models;
public class HaSetting
{
    public string BaseUrl
    {
        get; set;
    } = "http://localhost:8123";

    public string HaToken
    {
        get; set;
    } = "";
}
