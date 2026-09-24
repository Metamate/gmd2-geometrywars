using GeometryWars6.Graphics;
using GMDCore;
using GMDCore.States;
using GeometryWars6.Input;
using GeometryWars6.Services;
using GeometryWars6.Systems;
using GeometryWars6.States;
using Microsoft.Xna.Framework;

namespace GeometryWars6;

public sealed class Game1 : Core
{
    private readonly BloomComponent _bloom;
    private FrameInfo Frame { get; } = new();
    private GameAssets Assets { get; } = new();
    private AudioManager Audio { get; } = new();
    private PerformanceMonitor Performance { get; } = new();
    private GameController Controller { get; }
    public PlayContext PlayContext { get; }

    public Game1() : base("Geometry Wars", GameSettings.Window.Width, GameSettings.Window.Height,
               GameSettings.Window.Width, GameSettings.Window.Height)
    {
        Controller = new GameController(Input);
        PlayContext = new PlayContext(Frame, Controller, Assets, Audio, Performance);
        Graphics.SynchronizeWithVerticalRetrace = false;
        IsFixedTimeStep = false;
        IsMouseVisible = false;
        Window.AllowUserResizing = false;
        StateStack = new StateStack();

        _bloom = new BloomComponent(this);
        Components.Add(_bloom);
        _bloom.Settings = new BloomSettings("", 0.2f, 4f, 2f, 1f, 1.5f, 1f);
    }

    protected override void Initialize()
    {
        base.Initialize();

        Frame.Update(new GameTime(), GraphicsDevice.Viewport);

        StateStack.Push(new PlayState(this, PlayContext));
    }

    protected override void LoadContent()
    {
        Assets.Load(Content);
    }

    protected override void UpdateGame(GameTime gameTime)
    {
        Controller.Update();
        Frame.Update(gameTime, GraphicsDevice.Viewport);
        StateStack.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        Performance.Update(gameTime);
        GraphicsDevice.Clear(Color.Black);
        _bloom.BeginDraw();
        StateStack.Draw(SpriteBatch);
        base.Draw(gameTime);
        StateStack.DrawHUD(SpriteBatch);
    }
}
