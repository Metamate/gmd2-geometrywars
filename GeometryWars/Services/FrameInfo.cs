using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Services;

// Per-frame runtime information captured by the game loop and shared explicitly.
public sealed class FrameInfo
{
    public GameTime Time { get; private set; } = new();
    public Viewport Viewport { get; private set; }

    public Vector2 ScreenSize => new(Viewport.Width, Viewport.Height);
    public float TotalSeconds => (float)Time.TotalGameTime.TotalSeconds;
    public float ElapsedSeconds => (float)Time.ElapsedGameTime.TotalSeconds;

    public void Update(GameTime time, Viewport viewport)
    {
        Time = time;
        Viewport = viewport;
    }
}
