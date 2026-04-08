using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Components;

/// <summary>
/// Draws a secondary texture centered on the entity.
/// Demonstrates visual overlays as components.
/// </summary>
public sealed class GlowOverlay : IComponent, IDrawableComponent
{
    private readonly Texture2D _texture;
    private readonly Color _color;

    public GlowOverlay(Texture2D texture, Color color)
    {
        _texture = texture;
        _color   = color;
    }

    public void Update(Entity owner) { }

    public void Draw(Entity owner, SpriteBatch spriteBatch)
    {
        Vector2 origin = new(_texture.Width / 2f, _texture.Height / 2f);
        
        // PERFORMANCE: Direct cached access to Orientation
        float orientation = owner.Movement?.Orientation ?? 0f;
        
        spriteBatch.Draw(_texture, owner.Position, null, _color, orientation, origin, 1f, 0, 0);
    }
}
