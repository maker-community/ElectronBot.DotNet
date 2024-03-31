using ElectronBot.Braincase;
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
}
