using GMDCore.Input;
using GeometryWars.Input;
using GeometryWars.Systems;

namespace GeometryWars.Services;

// Aggregates runtime dependencies that used to live in static globals.
public sealed class GameRuntime
{
    public InputManager Input { get; }
    public GameController Controller { get; }
    public FrameInfo Frame { get; } = new();
    public GameAssets Assets { get; } = new();
    public AudioManager Audio { get; } = new();
    public PerformanceMonitor Performance { get; } = new();

    public GameRuntime(InputManager input)
    {
        Input = input;
        Controller = new GameController(input);
    }
}
