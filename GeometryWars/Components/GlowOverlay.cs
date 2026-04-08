using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Components;

public sealed class GlowOverlay : IComponent, IDrawableComponent
{
    private readonly Texture2D _texture;
    private readonly Color _color;
    private TransformComponent _transform;

    public GlowOverlay(Texture2D texture, Color color)
    {
        _texture = texture;
        _color   = color;
    }

    public void OnAdded(Entity owner)
    {
        _transform = owner.GetComponent<TransformComponent>();
    }

    public void Update(Entity owner) { }

    public void Draw(Entity owner, SpriteBatch spriteBatch)
    {
        if (_transform == null) return;

        Vector2 origin = new(_texture.Width / 2f, _texture.Height / 2f);
        spriteBatch.Draw(_texture, _transform.Position, null, _color, _transform.Orientation, origin, _transform.Scale, SpriteEffects.None, 0f);
    }
}
