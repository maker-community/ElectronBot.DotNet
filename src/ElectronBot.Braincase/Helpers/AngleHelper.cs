using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace ElectronBot.Braincase.Helpers;

public static class AngleHelper
{
    public static float GetPointAngle(
        Vector2 startVector2, Vector2 endVector2, Vector2 vertex)
    {
        var p1 = vertex;
        var p2 = startVector2;
        var p3 = endVector2;

        var v1 = p2 - p1;
        var v2 = p3 - p1;

        var dotProduct = Vector2.Dot(v1, v2);
        var v1Magnitude = v1.Length();
        var v2Magnitude = v2.Length();
        var angle = MathF.Acos(dotProduct / (v1Magnitude * v2Magnitude)) * 180 / MathF.PI;
        return angle;
    }
}
