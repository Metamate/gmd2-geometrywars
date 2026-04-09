using GeometryWars.Graphics;
using GMDCore;
using GeometryWars.Services;
using GeometryWars.States;
using Microsoft.Xna.Framework;

namespace GeometryWars;

public sealed class Game1 : Core
{
    private readonly BloomComponent _bloom;
    private GameRuntime Runtime { get; }
    public PlayContext PlayContext { get; }

    public Game1() : base(GameSettings.Window.Width, GameSettings.Window.Height)
    {
        Runtime = new GameRuntime(Input);
        PlayContext = new PlayContext(Runtime.Frame, Runtime.Controller, Runtime.Assets, Runtime.Audio, Runtime.Performance);
        Graphics.SynchronizeWithVerticalRetrace = false;
        IsFixedTimeStep = false;

        _bloom = new BloomComponent(this);
        Components.Add(_bloom);
        _bloom.Settings = new BloomSettings("", 0.2f, 4f, 2f, 1f, 1.5f, 1f);
    }

    protected override void Initialize()
    {
        base.Initialize();

        Runtime.Frame.Update(new GameTime(), GraphicsDevice.Viewport);

        SetState(new PlayState(this, PlayContext));
    }

    protected override void Update(GameTime gameTime)
    {
        Runtime.Performance.Update(gameTime);
        Runtime.Input.Mouse.UpdatePositionOnly();

        base.Update(gameTime);
    }

    protected override void LoadContent()
    {
        Runtime.Assets.Load(Content);
    }

    protected override void OnUpdateInput()
    {
        base.OnUpdateInput();
        Runtime.Controller.Update();
    }

    protected override bool ShouldExit() => Runtime.Controller.WasExitPressed;

    protected override void RegisterServices(GameTime gameTime)
    {
        Runtime.Frame.Update(gameTime, GraphicsDevice.Viewport);
    }

    protected override void OnBeginDraw() => _bloom.BeginDraw();
}
