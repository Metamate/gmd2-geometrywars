using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GeometryWars.Core;

// Base game class that owns the state machine.
// Subclasses override BuildContext() to supply frame-specific systems.
// Subclasses that need post-processing (e.g. bloom) override Draw() and call
// RunComponents() in place of base.Draw() to avoid double-invoking state drawing.
public abstract class GameCore : Game
{
    protected GraphicsDeviceManager Graphics;
    public SpriteBatch SpriteBatch { get; private set; }

    private GameStateBase _activeState;
    private GameContext _context;

    protected GameContext Context => _context;
    protected GameStateBase ActiveState => _activeState;

    protected GameCore(int width, int height)
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

    protected abstract GameContext BuildContext(GameTime gameTime);

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Input.Update();

        _context = BuildContext(gameTime);
        _activeState?.Update(_context);

        base.Update(gameTime);
    }

    // Runs Game.Draw() → Components.Draw() (bloom, etc.) without re-invoking state methods.
    // Call this from an overriding Draw() instead of base.Draw() to avoid double-draw.
    protected void RunComponents(GameTime gameTime) => base.Draw(gameTime);

    // Default draw path for games without post-processing.
    // Override Draw() and use RunComponents() if you need to wrap world rendering.
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
        _activeState?.DrawWorld(SpriteBatch, _context);
        RunComponents(gameTime);
        _activeState?.DrawHUD(SpriteBatch, _context);
    }
}
