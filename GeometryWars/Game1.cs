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

    public Game1() : base(GameSettings.ScreenWidth, GameSettings.ScreenHeight)
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

        // Register stable services once — these are created here and never replaced.
        // Frame-varying services (Time, Viewport) are updated in RegisterServices().
        GameServices.Particles = new ParticleManager<ParticleState>(GameSettings.MaxParticles, ParticleState.UpdateParticle);

        Vector2 gridSpacing = new(MathF.Sqrt(GraphicsDevice.Viewport.Width * GraphicsDevice.Viewport.Height / GameSettings.MaxGridPoints));
        GameServices.Grid = new Grid(GraphicsDevice.Viewport.Bounds, gridSpacing);

        // Seed frame-varying services before the first Update tick.
        RegisterServices(new GameTime());

        SetState(new PlayState(this));
    }

    protected override void Update(GameTime gameTime)
    {
        // Performance and Mouse position (for UI) are updated every rendering frame.
        GameServices.Performance.Update(gameTime);
        Input.UpdateMouseOnly();

        base.Update(gameTime);
    }

    protected override void LoadContent()
    {
        Art.Load(Content);
        Sound.Load(Content);
    }

    // The main logic tick calls the full input update (keyboard, gamepad, mouse history).
    protected override void OnUpdateInput() => Input.Update();

    // Called once per frame — only update values that actually change each frame.
    // Stable services (Particles, Grid) stay set from Initialize().
    protected override void RegisterServices(GameTime gameTime)
    {
        GameServices.Time     = gameTime;
        GameServices.Viewport = GraphicsDevice.Viewport;
    }

    // Inserts bloom around the world draw pass.
    // RunComponents() triggers Game.Draw() → Components.Draw() (bloom post-processing)
    // without re-invoking state drawing that GameCore.Draw() would add.
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
        _bloom.BeginDraw();
        ActiveState?.DrawWorld(SpriteBatch);
        RunComponents(gameTime);
        ActiveState?.DrawHUD(SpriteBatch);
    }
}
