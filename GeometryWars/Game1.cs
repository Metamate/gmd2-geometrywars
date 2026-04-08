using System;
using BloomPostprocess;
using GMDCore;
using GeometryWars.Services;
using GeometryWars.States;
using Microsoft.Xna.Framework;

namespace GeometryWars;

public class Game1 : Core
{
    private readonly BloomComponent _bloom;

    public Game1() : base(GameSettings.Window.Width, GameSettings.Window.Height)
    {
        Graphics.SynchronizeWithVerticalRetrace = false;
        IsFixedTimeStep = false;

        _bloom = new BloomComponent(this);
        Components.Add(_bloom);
        _bloom.Settings = new BloomSettings("", 0.2f, 4f, 2f, 1f, 1.5f, 1f);
    }

    protected override void Initialize()
    {
        base.Initialize();

        GameServices.Particles = new ParticleManager<ParticleState>(GameSettings.Performance.MaxParticles, ParticleState.UpdateParticle);

        Vector2 gridSpacing = new(MathF.Sqrt(GraphicsDevice.Viewport.Width * GraphicsDevice.Viewport.Height / GameSettings.Performance.MaxGridPoints));
        GameServices.Grid = new Grid(GraphicsDevice.Viewport.Bounds, gridSpacing);

        RegisterServices(new GameTime());

        SetState(new PlayState(this));
    }

    protected override void Update(GameTime gameTime)
    {
        GameServices.Performance.Update(gameTime);
        Input.Mouse.UpdatePositionOnly();

        base.Update(gameTime);
    }

    protected override void LoadContent()
    {
        Art.Load(Content);
        Sound.Load(Content);
    }

    protected override void OnUpdateInput()
    {
        base.OnUpdateInput();
        GameController.Update();
    }

    protected override bool ShouldExit() => GameController.WasExitPressed;

    protected override void RegisterServices(GameTime gameTime)
    {
        FrameContext.Time = gameTime;
        FrameContext.Viewport = GraphicsDevice.Viewport;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _bloom.BeginDraw();
        ActiveState?.DrawWorld(SpriteBatch);
        RunComponents(gameTime);
        ActiveState?.DrawHUD(SpriteBatch);
    }
}
