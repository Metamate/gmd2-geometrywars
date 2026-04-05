using Microsoft.Xna.Framework.Graphics;

namespace GMDCore;

// Base for all top-level game states.
// DrawWorld renders before post-processing (e.g. bloom); DrawHUD renders after.
public abstract class GameStateBase
{
    public virtual void Enter() { }
    public virtual void Exit() { }
    public abstract void Update();
    public virtual void DrawWorld(SpriteBatch spriteBatch) { }
    public virtual void DrawHUD(SpriteBatch spriteBatch) { }
}
