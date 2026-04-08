using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Components;

public interface IDrawableComponent
{
    void Draw(Entity owner, SpriteBatch spriteBatch);
}
