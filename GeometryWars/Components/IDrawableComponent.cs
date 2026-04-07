using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Components;

// Optional companion to IComponent for components that need to render.
// Entity.Draw() calls Draw() on any attached component that implements this interface,
// after the entity's own sprite is drawn. This lets components add visual overlays
// (glows, shields, debug shapes) without subclassing the entity.
//
// Example: attach a GlowOverlay to any entity to draw a secondary texture on top.
// Example: attach a ColliderDebugRenderer to any entity to visualise its hit radius.
public interface IDrawableComponent
{
    void Draw(Entity owner, SpriteBatch spriteBatch);
}
