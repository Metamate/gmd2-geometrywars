using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

// Holds all game-wide runtime state. Created each frame by Game1 and passed through Update().
// Replaces scattered Game1.Instance / Game1.GameTime / Game1.ParticleManager references.
// GameContext.Current is also available for static delegates (e.g. ParticleState.UpdateParticle).
public sealed class GameContext
{
    public static GameContext Current { get; private set; }

    public GameTime GameTime { get; private init; }
    public ParticleManager<ParticleState> Particles { get; private init; }
    public Grid Grid { get; private init; }
    public Viewport Viewport { get; private init; }

    public Vector2 ScreenSize => new(Viewport.Width, Viewport.Height);
    public double TotalSeconds => GameTime.TotalGameTime.TotalSeconds;
    public float ElapsedSeconds => (float)GameTime.ElapsedGameTime.TotalSeconds;

    public GameContext(GameTime gameTime, ParticleManager<ParticleState> particles, Grid grid, Viewport viewport)
    {
        GameTime = gameTime;
        Particles = particles;
        Grid = grid;
        Viewport = viewport;
        Current = this;
    }
}
