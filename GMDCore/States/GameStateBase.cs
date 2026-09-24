using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GMDCore.States;

// Base class for all game states managed by the StateStack.
// Each state owns its own Begin/End calls inside Draw(), usually through Core.BeginDraw().
// DrawHUD() draws on top of everything, after any post-processing (e.g. bloom).
// A state overrides whichever of the two it needs.
public abstract class GameStateBase
{
    public virtual void Enter() { }
    public virtual void Exit()  { }
    public virtual void Update(GameTime gameTime) { }
    public virtual void Draw(SpriteBatch spriteBatch) { }
    public virtual void DrawHUD(SpriteBatch spriteBatch) { }
}
