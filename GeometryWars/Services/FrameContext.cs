using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Services;

/// <summary>
/// Data that is only valid for the current frame.
/// Refreshed every tick in Game1.RegisterServices().
/// </summary>
public static class FrameContext
{
    public static GameTime Time { get; set; }
    public static Viewport Viewport { get; set; }

    public static Vector2 ScreenSize => new(Viewport.Width, Viewport.Height);
    public static double TotalSeconds => Time.TotalGameTime.TotalSeconds;
    public static float ElapsedSeconds => (float)Time.ElapsedGameTime.TotalSeconds;
}
