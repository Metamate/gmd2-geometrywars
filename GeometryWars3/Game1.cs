using GMDCore;
using GeometryWars3.Input;
using GeometryWars3.Services;
using GeometryWars3.Systems;
using GeometryWars3.States;
using Microsoft.Xna.Framework;

namespace GeometryWars3;

public sealed class Game1 : Core
{
    private FrameInfo Frame { get; } = new();
    private GameAssets Assets { get; } = new();
    private PerformanceMonitor Performance { get; } = new();
    private GameController Controller { get; }
    public PlayContext PlayContext { get; }

    public Game1() : base(GameSettings.Window.Width, GameSettings.Window.Height)
    {
        Controller = new GameController(Input);
        PlayContext = new PlayContext(Frame, Controller, Assets, Performance);
        Graphics.SynchronizeWithVerticalRetrace = false;
        IsFixedTimeStep = false;
    }

    protected override void Initialize()
    {
        base.Initialize();

        Frame.Update(new GameTime(), GraphicsDevice.Viewport);

        SetState(new PlayState(this, PlayContext));
    }

    protected override void Update(GameTime gameTime)
    {
        Performance.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void LoadContent()
    {
        Assets.Load(Content);
    }

    protected override void OnUpdateInput()
    {
        base.OnUpdateInput();
        Controller.Update();
    }

    protected override bool ShouldExit() => Controller.WasExitPressed;

    protected override void RegisterServices(GameTime gameTime)
    {
        Frame.Update(gameTime, GraphicsDevice.Viewport);
    }
}
