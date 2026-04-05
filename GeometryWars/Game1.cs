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
    private ParticleManager<ParticleState> _particles;
    private Grid _grid;

    public Game1() : base(GameSettings.ScreenWidth, GameSettings.ScreenHeight)
    {
        _bloom = new BloomComponent(this);
        Components.Add(_bloom);
        _bloom.Settings = new BloomSettings("", 0.2f, 4f, 2f, 1f, 1.5f, 1f);
    }

    protected override void Initialize()
    {
        base.Initialize();

        _particles = new ParticleManager<ParticleState>(GameSettings.MaxParticles, ParticleState.UpdateParticle);

        Vector2 gridSpacing = new(MathF.Sqrt(GraphicsDevice.Viewport.Width * GraphicsDevice.Viewport.Height / GameSettings.MaxGridPoints));
        _grid = new Grid(GraphicsDevice.Viewport.Bounds, gridSpacing);

        // Seed services for the first frame before the first Update tick
        RegisterServices(new GameTime());

        SetState(new PlayState(this));
    }

    protected override void LoadContent()
    {
        Art.Load(Content);
        Sound.Load(Content);
    }

    protected override void OnUpdateInput() => Input.Update();

    // Push game-specific singletons into the service locator each frame.
    // Time and Viewport change each frame; Particles and Grid are stable after init.
    protected override void RegisterServices(GameTime gameTime)
    {
        GameServices.Time = gameTime;
        GameServices.Viewport = GraphicsDevice.Viewport;
        GameServices.Particles = _particles;
        GameServices.Grid = _grid;
    }

    // Inserts bloom around the world draw pass.
    // RunComponents() triggers Game.Draw() -> Components.Draw() (bloom post-processing)
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
