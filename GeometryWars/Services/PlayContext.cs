using GeometryWars.Input;
using GeometryWars.Systems;

namespace GeometryWars.Services;

// The subset of runtime dependencies needed by the active play session and states.
public sealed class PlayContext
{
    public FrameInfo Frame { get; }
    public GameController Controller { get; }
    public GameAssets Assets { get; }
    public AudioManager Audio { get; }
    public PerformanceMonitor Performance { get; }

    public PlayContext(FrameInfo frame, GameController controller, GameAssets assets, AudioManager audio, PerformanceMonitor performance)
    {
        Frame = frame;
        Controller = controller;
        Assets = assets;
        Audio = audio;
        Performance = performance;
    }
}
