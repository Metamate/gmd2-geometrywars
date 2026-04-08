using GeometryWars.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Components;

/// <summary>
/// Component that renders a sprite. Owns the visual-only 'Tint' data.
/// </summary>
public sealed class SpriteComponent : IComponent, IDrawableComponent
{
    private readonly Texture2D _texture;
    private TransformComponent _transform;
    
    public Color Tint { get; set; } = Color.White;

    public SpriteComponent(Texture2D texture)
    {
        _texture = texture;
    }

    public void OnAdded(Entity owner)
    {
        _transform = owner.Transform;
    }

    public void Update(Entity owner) { }

    public void Draw(Entity owner, SpriteBatch spriteBatch)
    {
        _transform ??= owner.Transform;
        if (_transform == null) return;

        Vector2 origin = new(_texture.Width / 2f, _texture.Height / 2f);

        spriteBatch.Draw(_texture, _transform.Position, null, Tint, 
            _transform.Orientation, origin, _transform.Scale, SpriteEffects.None, 0f);
    }
}
