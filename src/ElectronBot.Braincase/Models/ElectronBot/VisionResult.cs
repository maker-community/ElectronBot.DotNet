using ElectronBot.Braincase;
using Mediapipe.Net.Framework.Protobuf;
using Mediapipe.Net.Solutions;

namespace Models.ElectronBot;

public class VisionResult
{
    public PoseOutput? PoseOutput
    {
        get; set;
    }

    public string? HandResult
    {
        get; set;
    }

    public Emoji? Emoji
    {
        get; set;
    }

    public NormalizedLandmarkList? PoseLandmarks
    {
        get; set;
    }

    public  int Width
    {
        get; set;
    }

    public int Height
    {
        get; set;
    }
}
