using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Utils;

public static class Extensions
{
    // Draws a line between two points using a scaled 1x1 pixel texture.
    public static void DrawLine(this SpriteBatch spriteBatch, Texture2D pixel, Vector2 start, Vector2 end, Color color, float thickness = 2f)
    {
        Vector2 delta = end - start;
        spriteBatch.Draw(pixel, start, null, color, delta.ToAngle(), new Vector2(0, 0.5f), new Vector2(delta.Length(), thickness), SpriteEffects.None, 0f);
    }

    // Returns the angle (in radians) that this vector points toward.
    public static float ToAngle(this Vector2 vector)
    {
        return (float)Math.Atan2(vector.Y, vector.X);
    }

    // Returns a random float uniformly distributed in [minValue, maxValue).
    public static float NextFloat(this Random rand, float minValue, float maxValue)
    {
        return (float)rand.NextDouble() * (maxValue - minValue) + minValue;
    }

    // Returns a random Vector2 with a uniformly random direction and length in [minLength, maxLength).
    public static Vector2 NextVector2(this Random rand, float minLength, float maxLength)
    {
        double theta = rand.NextDouble() * 2 * Math.PI;
        float length = rand.NextFloat(minLength, maxLength);
        return new Vector2(length * (float)Math.Cos(theta), length * (float)Math.Sin(theta));
    }

    // Returns this vector scaled to exactly the given length.
    public static Vector2 ScaleTo(this Vector2 vector, float length)
    {
        float lengthSq = vector.LengthSquared();
        if (lengthSq < 0.000001f)
            return Vector2.Zero;

        return vector * (length / MathF.Sqrt(lengthSq));
    }
}
