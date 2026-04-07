using System;
using BloomPostprocess;
using GMDCore;
using GeometryWars.Services;
using GeometryWars.States;
using Microsoft.Xna.Framework;

namespace GeometryWars;

public class Game1 : GameCore
{
    private readonly BloomComponent _bloom;

    public Game1() : base(GameSettings.Window.Width, GameSettings.Window.Height)
    {
        // Disable VSync and FixedTimeStep for performance benchmarking.
        Graphics.SynchronizeWithVerticalRetrace = false;
        IsFixedTimeStep = false;

        _bloom = new BloomComponent(this);
        Components.Add(_bloom);
        _bloom.Settings = new BloomSettings("", 0.2f, 4f, 2f, 1f, 1.5f, 1f);
    }

    protected override void Initialize()
    {
        base.Initialize();

        // Register stable services once.
        GameServices.Particles = new ParticleManager<ParticleState>(GameSettings.Performance.MaxParticles, ParticleState.UpdateParticle);

        Vector2 gridSpacing = new(MathF.Sqrt(GraphicsDevice.Viewport.Width * GraphicsDevice.Viewport.Height / GameSettings.Performance.MaxGridPoints));
        GameServices.Grid = new Grid(GraphicsDevice.Viewport.Bounds, gridSpacing);

        // Seed frame context before the first tick.
        RegisterServices(new GameTime());

        SetState(new PlayState(this));
    }

    protected override void Update(GameTime gameTime)
    {
        // Performance and Mouse (UI) update every frame for smoothness.
        GameServices.Performance.Update(gameTime);
        RawInput.UpdateMouseOnly();

        base.Update(gameTime);
    }

    protected override void LoadContent()
    {
        Art.Load(Content);
        Sound.Load(Content);
    }

    // The main logic tick (60Hz) handles the core input processing.
    protected override void OnUpdateInput()
    {
        RawInput.Update();
        GameController.Update();
    }

    protected override bool ShouldExit() => GameController.WasExitPressed;

    protected override void RegisterServices(GameTime gameTime)
    {
        // Refresh temporal frame-specific data.
        FrameContext.Time     = gameTime;
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
