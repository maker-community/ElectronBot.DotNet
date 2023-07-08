namespace Verdure.ElectronBot.Core.Models;
public class ModelActionFrame
{
    public Stream FrameStream
    {
        get; set;
    }
    public float J1
    {
        get; set;
    }
    public float J2
    {
        get; set;
    }
    public float J3
    {
        get; set;
    }
    public float J4
    {
        get; set;
    }
    public float J5
    {
        get; set;
    }
    public float J6
    {
        get; set;
    }
    public bool Enable
    {
        get; set;
    }
    public ModelActionFrame(
        Stream imgData, 
        bool enable = false, 
        float j1 = 0, 
        float j2 = 0, 
        float j3 = 0, 
        float j4 = 0, 
        float j5 = 0, 
        float j6 = 0)
    {
        FrameStream = imgData;
        J1 = j1;
        J2 = j2;
        J3 = j3;
        J4 = j4;
        J5 = j5;
        J6 = j6;
        Enable = enable;
    }

    public OnlyAction Actions
    {
        get;
        set;
    } = new OnlyAction();
}


public class OnlyAction
{
    public OnlyAction()
    {
        
    }

    public OnlyAction(List<float> actions)
    {
        if (actions != null && actions.Count>0)
        {
            J1 = actions[0];
            J2 = actions[1];
            J3 = actions[2];
            J4 = actions[3];
            J5 = actions[4];
            J6 = actions[5];
        }
    }

    public float J1
    {
        get; set;
    }
    public float J2
    {
        get; set;
    }
    public float J3
    {
        get; set;
    }
    public float J4
    {
        get; set;
    }
    public float J5
    {
        get; set;
    }
    public float J6
    {
        get; set;
    }
}
