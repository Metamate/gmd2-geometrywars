using GeometryWars.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Services;

// Service locator for game-wide singletons.
//
// Stable services — set once in Game1.Initialize(), never change at runtime:
//   Particles   : particle system for visual effects
//   Grid        : spring-mass background distortion grid
//   Performance : frames per second and memory monitor
//
// Frame-varying services — refreshed every frame in Game1.RegisterServices():
//   Time      : current GameTime (elapsed, total)
//   Viewport  : current screen viewport (width, height, bounds)
//
// Access pattern: any class can read from GameServices without being coupled
// to the specific object that owns the data. Avoids threading constructor
// parameters through every layer of the call stack.
public static class GameServices
{
    public static ParticleManager<ParticleState> Particles   { get; set; }
    public static Grid                           Grid        { get; set; }
    public static GameTime                       Time        { get; set; }
    public static Viewport                       Viewport    { get; set; }
    public static PerformanceMonitor             Performance { get; set; } = new();

    public static Vector2 ScreenSize    => new(Viewport.Width, Viewport.Height);
    public static double  TotalSeconds  => Time.TotalGameTime.TotalSeconds;
    public static float   ElapsedSeconds => (float)Time.ElapsedGameTime.TotalSeconds;
}
