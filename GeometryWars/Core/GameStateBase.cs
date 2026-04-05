using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Core;

// Abstract base for all top-level game states (Play, GameOver, etc.).
// DrawWorld is rendered before post-processing (bloom); DrawHUD after.
public abstract class GameStateBase
{
    public virtual void Enter() { }
    public virtual void Exit() { }
    public abstract void Update(GameContext ctx);
    public virtual void DrawWorld(SpriteBatch spriteBatch, GameContext ctx) { }
    public virtual void DrawHUD(SpriteBatch spriteBatch, GameContext ctx) { }
}
