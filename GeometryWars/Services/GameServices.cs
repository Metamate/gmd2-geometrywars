using GeometryWars.Systems;

namespace GeometryWars.Services;

/// <summary>
/// Service locator for stable, game-wide systems.
/// Initialized once in Game1.Initialize().
/// </summary>
public static class GameServices
{
    public static ParticleManager<ParticleState> Particles { get; set; }
    public static Grid Grid { get; set; }
    public static PerformanceMonitor Performance { get; set; } = new();
}
