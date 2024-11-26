namespace Verdure.IoT.Net.Models;
public class HaState
{
    public string entity_id
    {
        get; set;
    }
    public string state
    {
        get; set;
    }
    public Attributes attributes
    {
        get; set;
    }
    public DateTime last_changed
    {
        get; set;
    }
    public DateTime last_updated
    {
        get; set;
    }
    public Context context
    {
        get; set;
    }
}

public class Attributes
{
    public DateTime next_dawn
    {
        get; set;
    }
    public DateTime next_dusk
    {
        get; set;
    }
    public DateTime next_midnight
    {
        get; set;
    }
    public DateTime next_noon
    {
        get; set;
    }
    public DateTime next_rising
    {
        get; set;
    }
    public DateTime next_setting
    {
        get; set;
    }
    public float elevation
    {
        get; set;
    }
    public float azimuth
    {
        get; set;
    }
    public bool rising
    {
        get; set;
    }
    public string friendly_name
    {
        get; set;
    }
    public float latitude
    {
        get; set;
    }
    public float longitude
    {
        get; set;
    }
    public int radius
    {
        get; set;
    }
    public bool passive
    {
        get; set;
    }
    public bool editable
    {
        get; set;
    }
    public string icon
    {
        get; set;
    }
    public string release_notes
    {
        get; set;
    }
    public string newest_version
    {
        get; set;
    }
    public string device_class
    {
        get; set;
    }
    public string id
    {
        get; set;
    }
    public string user_id
    {
        get; set;
    }
    public string title
    {
        get; set;
    }
    public string message
    {
        get; set;
    }
}

public class Context
{
    public string id
    {
        get; set;
    }
    public object parent_id
    {
        get; set;
    }
    public object user_id
    {
        get; set;
    }
}
