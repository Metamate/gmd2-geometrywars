using System;
using Microsoft.Xna.Framework;

namespace GeometryWars.Utils;

public static class MathUtil
{
    // Converts polar coordinates (angle in radians, magnitude) to a Cartesian Vector2.
    // Angle 0 points right (+X), increasing clockwise in screen space.
    public static Vector2 FromPolar(float angle, float magnitude)
    {
        return magnitude * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
    }
}
