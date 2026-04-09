using GeometryWars.Components.Core;
using GeometryWars.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Components.Visuals;

// Renders a texture at the entity's position.
public sealed class Sprite : Component
{
    private readonly Texture2D _texture;
    private Transform _transform;

    public Color Tint { get; set; } = Color.White;

    public Sprite(Texture2D texture)
    {
        _texture = texture;
    }

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
    }

    public override void Draw(Entity owner, SpriteBatch spriteBatch)
    {
        Vector2 origin = new(_texture.Width / 2f, _texture.Height / 2f);

        spriteBatch.Draw(_texture, _transform.Position, null, Tint, 
            _transform.Orientation, origin, _transform.Scale, SpriteEffects.None, 0f);
    }
}
