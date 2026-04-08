using GeometryWars.Entities;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Components.Visuals;

/// <summary>
/// Opt-in interface for components that need to render.
/// Entity.Draw() calls Draw() on every active component that implements this interface.
/// </summary>
public interface IDrawableComponent
{
    void Draw(Entity owner, SpriteBatch spriteBatch);
}
