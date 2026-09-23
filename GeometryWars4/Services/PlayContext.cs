using GeometryWars4.Input;
using GeometryWars4.Systems;

namespace GeometryWars4.Services;

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
