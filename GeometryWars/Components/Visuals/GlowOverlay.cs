using GMDCore.ECS.Components;
using GMDCore.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Components.Visuals;

// Renders a glow effect centered on the entity.
public sealed class GlowOverlay : Component
{
    private readonly Texture2D _texture;
    private readonly Color _color;
    private Transform _transform;

    public GlowOverlay(Texture2D texture, Color color)
    {
        _texture = texture;
        _color   = color;
    }

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
    }

    public override void Draw(Entity owner, SpriteBatch spriteBatch)
    {
        Vector2 origin = new(_texture.Width / 2f, _texture.Height / 2f);
        spriteBatch.Draw(_texture, _transform.Position, null, _color, _transform.Orientation, origin, _transform.Scale, SpriteEffects.None, 0f);
    }
}

