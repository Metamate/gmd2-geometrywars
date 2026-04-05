using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Services;

// Service locator for game-wide singletons.
// Game1.RegisterServices() refreshes Time and Viewport each frame.
// Particles and Grid are set once during initialization.
public static class GameServices
{
    public static ParticleManager<ParticleState> Particles { get; set; }
    public static Grid Grid { get; set; }
    public static GameTime Time { get; set; }
    public static Viewport Viewport { get; set; }

    public static Vector2 ScreenSize => new(Viewport.Width, Viewport.Height);
    public static double TotalSeconds => Time.TotalGameTime.TotalSeconds;
    public static float ElapsedSeconds => (float)Time.ElapsedGameTime.TotalSeconds;
}
