using GeometryWars.Systems;

namespace GeometryWars.Services;

/// <summary>
/// Service locator for stable, game-wide systems.
/// Call Initialize() once in Game1.Initialize() before the game loop starts.
/// </summary>
public static class GameServices
{
    public static ParticleManager<ParticleState> Particles { get; private set; }
    public static Grid Grid { get; private set; }
    public static PerformanceMonitor Performance { get; private set; } = new();
    public static AudioManager Audio { get; private set; } = new();

    public static void Initialize(ParticleManager<ParticleState> particles, Grid grid)
    {
        Particles = particles;
        Grid = grid;
    }
}
