using GMDCore;
using GMDCore.States;
using GeometryWars4.Input;
using GeometryWars4.Services;
using GeometryWars4.Systems;
using GeometryWars4.States;
using Microsoft.Xna.Framework;

namespace GeometryWars4;

public sealed class Game1 : Core
{
    private FrameInfo Frame { get; } = new();
    private GameAssets Assets { get; } = new();
    private PerformanceMonitor Performance { get; } = new();
    private GameController Controller { get; }
    public PlayContext PlayContext { get; }

    public Game1() : base("Geometry Wars", GameSettings.Window.Width, GameSettings.Window.Height,
               GameSettings.Window.Width, GameSettings.Window.Height)
    {
        Controller = new GameController(Input);
        PlayContext = new PlayContext(Frame, Controller, Assets, Performance);
        Graphics.SynchronizeWithVerticalRetrace = false;
        IsFixedTimeStep = false;
        IsMouseVisible = false;
        Window.AllowUserResizing = false;
        StateStack = new StateStack();
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
        StateStack.Draw(SpriteBatch);
        base.Draw(gameTime);
        StateStack.DrawHUD(SpriteBatch);
    }
}
