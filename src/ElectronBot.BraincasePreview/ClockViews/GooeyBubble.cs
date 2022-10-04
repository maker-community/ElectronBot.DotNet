using System;
using System.Collections.Generic;
using System.Text;
using ElectronBot.BraincasePreview.AnimationTimelines;

namespace ElectronBot.BraincasePreview.ClockViews;

public class GooeyBubble
{
    public DoubleTimeline OffsetTimeline
    {
        get; set;
    }
    public DoubleTimeline SizeTimeline
    {
        get; set;
    }
    public double X
    {
        get; set;
    }
}