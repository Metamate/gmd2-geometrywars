using GeometryWars5.Graphics;
using GMDCore;
using GeometryWars5.Input;
using GeometryWars5.Services;
using GeometryWars5.Systems;
using GeometryWars5.States;
using Microsoft.Xna.Framework;

namespace GeometryWars5;

public sealed class Game1 : Core
{
    private readonly BloomComponent _bloom;
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

        _bloom = new BloomComponent(this);
        Components.Add(_bloom);
        _bloom.Settings = new BloomSettings("", 0.2f, 4f, 2f, 1f, 1.5f, 1f);
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

    protected override void OnBeforeDrawWorld() => _bloom.BeginDraw();
}
