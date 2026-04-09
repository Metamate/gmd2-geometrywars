using System;
using GMDCore.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GMDCore;

// Base game class that owns the state machine.
//
// Implementation Note: Fixed-Timestep Accumulator
// We implement our own logic loop to allow uncapped rendering (VSync off)
// while keeping gameplay simulation deterministic at 60Hz. This prevents
// physics (like the spring grid) from breaking at high frame rates.
public abstract class Core : Game
{
    protected GraphicsDeviceManager Graphics;
    public SpriteBatch SpriteBatch { get; private set; }
    public InputManager Input { get; } = new();

    private GameStateBase _activeState;
    private double _accumulator;
    private readonly GameTime _fixedGameTime = new();

    protected GameStateBase ActiveState => _activeState;

    protected Core(int width, int height)
    {
        Graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = width,
            PreferredBackBufferHeight = height,
        };
        Content.RootDirectory = "Content";
    }

    protected override void Initialize()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        base.Initialize();
    }

    public void SetState(GameStateBase newState)
    {
        _activeState?.Exit();
        _activeState = newState;
        _activeState?.Enter();
    }

    protected virtual void RegisterServices(GameTime gameTime) { }

    protected virtual void OnUpdateInput()
    {
        Input.Update();
    }

    // Override to provide a game-specific exit condition.
    // ShouldExit() is called after OnUpdateInput(), so Input is current.
    protected virtual bool ShouldExit()
        => Input.Keyboard.IsKeyDown(Keys.Escape) ||
           Input.GamePad.IsButtonDown(Buttons.Back);

    protected override void Update(GameTime gameTime)
    {
        if (!IsActive)
        {
            base.Update(gameTime);
            return;
        }

        // 60Hz fixed logic step
        const double timeStep = 1.0 / 60.0;
        _accumulator += gameTime.ElapsedGameTime.TotalSeconds;

        // Prevent "spiral of death" if the simulation falls too far behind
        if (_accumulator > 0.25) _accumulator = 0.25;

        while (_accumulator >= timeStep)
        {
            _fixedGameTime.ElapsedGameTime = TimeSpan.FromSeconds(timeStep);
            _fixedGameTime.TotalGameTime += TimeSpan.FromSeconds(timeStep);

            OnUpdateInput();
            if (ShouldExit()) { Exit(); return; }
            RegisterServices(_fixedGameTime);
            _activeState?.Update();

            _accumulator -= timeStep;
        }

        base.Update(gameTime);
    }

    // Triggers MonoGame's DrawableGameComponent pipeline (e.g. BloomComponent).
    // Must be called from Draw() after scene rendering is done.
    protected void RunComponents(GameTime gameTime) => base.Draw(gameTime);

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _activeState?.DrawWorld(SpriteBatch);
        RunComponents(gameTime);
        _activeState?.DrawHUD(SpriteBatch);
    }
}
