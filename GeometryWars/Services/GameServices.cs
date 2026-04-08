using GeometryWars.Systems;

namespace GeometryWars.Services;

/// <summary>
/// Service locator for stable, game-wide systems.
/// These services outlive any individual play session.
/// </summary>
public static class GameServices
{
    public static PerformanceMonitor Performance { get; private set; } = new();
    public static AudioManager Audio { get; private set; } = new();
}
