using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Components;

// Draws a secondary texture centered on the entity after its base sprite is drawn.
// Implements both IComponent and IDrawableComponent: Update() is a no-op (the glow
// does not change the entity's state), while Draw() adds the visual overlay.
//
// This demonstrates that a single component can span both the update and draw phases,
// and that drawable behaviour can be added to any entity without subclassing it.
//
// Usage: AddComponent(new GlowOverlay(Art.Glow, Color.Cyan * 0.6f));
public sealed class GlowOverlay : IComponent, IDrawableComponent
{
    private readonly Texture2D _texture;
    private readonly Color _color;

    public GlowOverlay(Texture2D texture, Color color)
    {
        _texture = texture;
        _color   = color;
    }

    // No per-frame state to update — the glow is purely visual.
    public void Update(Entity owner) { }

    public void Draw(Entity owner, SpriteBatch spriteBatch)
    {
        Vector2 origin = new(_texture.Width / 2f, _texture.Height / 2f);
        spriteBatch.Draw(_texture, owner.Position, null, _color, owner.Orientation, origin, 1f, 0, 0);
    }
}
