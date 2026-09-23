using GeometryWars3.Input;
using GeometryWars3.Systems;

namespace GeometryWars3.Services;

// The subset of runtime dependencies needed by the active play session and states.
public sealed class PlayContext
{
    public FrameInfo Frame { get; }
    public GameController Controller { get; }
    public GameAssets Assets { get; }
    public PerformanceMonitor Performance { get; }

    public PlayContext(FrameInfo frame, GameController controller, GameAssets assets, PerformanceMonitor performance)
    {
        Frame = frame;
        Controller = controller;
        Assets = assets;
        Performance = performance;
    }
}
