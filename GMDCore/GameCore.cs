using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GMDCore;

// Base game class that owns the state machine.
// Subclasses override RegisterServices() to push game-specific singletons into
// whatever service locator the game uses, called once per frame before state.Update().
// Subclasses that use post-processing override Draw() and use RunComponents() in
// place of base.Draw() to avoid double-invoking state drawing.
public abstract class GameCore : Game
{
    protected GraphicsDeviceManager Graphics;
    public SpriteBatch SpriteBatch { get; private set; }

    private GameStateBase _activeState;

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

    // Called once per frame before the active state updates.
    // Override to push this frame's GameTime, Viewport, etc. into a service locator.
    protected virtual void RegisterServices(GameTime gameTime) { }

    // Called once per frame to handle platform input (e.g. Input.Update()).
    protected virtual void OnUpdateInput() { }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        OnUpdateInput();
        RegisterServices(gameTime);
        _activeState?.Update();

        base.Update(gameTime);
    }

    // Runs Game.Draw() -> Components.Draw() (bloom etc.) without re-invoking state methods.
    // Call from an overriding Draw() instead of base.Draw() to avoid double-draw.
    protected void RunComponents(GameTime gameTime) => base.Draw(gameTime);

    // Default draw path for games without post-processing.
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
        _activeState?.DrawWorld(SpriteBatch);
        RunComponents(gameTime);
        _activeState?.DrawHUD(SpriteBatch);
    }
}
