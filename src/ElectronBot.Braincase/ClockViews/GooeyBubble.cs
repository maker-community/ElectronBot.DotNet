using System;
using System.Collections.Generic;
using System.Text;
using ElectronBot.Braincase.AnimationTimelines;

namespace ElectronBot.Braincase.ClockViews;

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