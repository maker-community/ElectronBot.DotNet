using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Hw75.YellowCalendar;

public class YellowCalendarData
{
    public string Reason
    {
        get; set;
    }
    public YellowCalendarResult Result
    {
        get; set;
    }
    public int error_code
    {
        get; set;
    }
}

public class YellowCalendarResult
{
    public string Id
    {
        get; set;
    }
    public string Yangli
    {
        get; set;
    }
    public string Yinli
    {
        get; set;
    }
    public string Wuxing
    {
        get; set;
    }
    public string Chongsha
    {
        get; set;
    }
    public string Baiji
    {
        get; set;
    }
    public string Jishen
    {
        get; set;
    }
    public string Yi
    {
        get; set;
    }
    public string Xiongshen
    {
        get; set;
    }
    public string Ji
    {
        get; set;
    }
}
