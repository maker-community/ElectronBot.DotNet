namespace Verdure.ElectronBot.Core.Models;
public class EmoticonActionFrame
{
    public byte[] FrameBuffer
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
    public EmoticonActionFrame(
        byte[] imgData, 
        bool enable = false, 
        float j1 = 0, 
        float j2 = 0, 
        float j3 = 0, 
        float j4 = 0, 
        float j5 = 0, 
        float j6 = 0)
    {
        FrameBuffer = imgData;
        J1 = j1;
        J2 = j2;
        J3 = j3;
        J4 = j4;
        J5 = j5;
        J6 = j6;
        Enable = enable;
    }
}
