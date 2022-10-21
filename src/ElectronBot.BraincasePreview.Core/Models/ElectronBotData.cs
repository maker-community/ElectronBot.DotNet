namespace Verdure.ElectronBot.Core.Models;

public class ElectronBotData
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
    public void SetJointAngles(float j1, float j2, float j3, float j4, float j5, float j6, bool enable = false)
    {
        J1 = j1;
        J2 = j2;
        J3 = j3;
        J4 = j4;
        J5 = j5;
        J6 = j6;
        Enable = enable;
    }

    public void SetImageSrc(byte[] data)
    {
        FrameBuffer = data;
    }
}
