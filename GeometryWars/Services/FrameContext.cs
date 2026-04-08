using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

public static class FrameContext
{
    public static GameTime Time { get; set; }
    public static Viewport Viewport { get; set; }

    public static Vector2 ScreenSize => new(Viewport.Width, Viewport.Height);
    public static float TotalSeconds => (float)Time.TotalGameTime.TotalSeconds;
}
