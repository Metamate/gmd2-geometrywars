using System;
using BloomPostprocess;
using GeometryWars.Core;
using GeometryWars.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

public class Game1 : GameCore
{
    private readonly BloomComponent _bloom;
    private ParticleManager<ParticleState> _particles;
    private Grid _grid;

    public Game1() : base(1920, 1080)
    {
        _bloom = new BloomComponent(this);
        Components.Add(_bloom);
        _bloom.Settings = new BloomSettings("", 0.2f, 4f, 2f, 1f, 1.5f, 1f);
    }

    protected override void Initialize()
    {
        base.Initialize(); // creates SpriteBatch, calls Components.Initialize()

        _particles = new ParticleManager<ParticleState>(1024 * 20, ParticleState.UpdateParticle);

        const int maxGridPoints = 1600;
        Vector2 gridSpacing = new(MathF.Sqrt(GraphicsDevice.Viewport.Width * GraphicsDevice.Viewport.Height / maxGridPoints));
        _grid = new Grid(GraphicsDevice.Viewport.Bounds, gridSpacing);

        SetState(new PlayState(this));
    }

    protected override void LoadContent()
    {
        Art.Load(Content);
        Sound.Load(Content);
    }

    protected override GameContext BuildContext(GameTime gameTime)
    {
        return new GameContext(gameTime, _particles, _grid, GraphicsDevice.Viewport);
    }

    // Overrides GameCore.Draw() so bloom can wrap world rendering.
    // RunComponents() triggers Components.Draw() (bloom post-processing) without
    // re-invoking state methods — avoids the double-draw that base.Draw() would cause.
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _bloom.BeginDraw();

        // World pass — state manages its own SpriteBatch Begin/End
        ActiveState?.DrawWorld(SpriteBatch, Context);

        // Bloom processes the render target
        RunComponents(gameTime);

        // HUD pass — drawn after bloom so UI stays crisp
        ActiveState?.DrawHUD(SpriteBatch, Context);
    }
}
