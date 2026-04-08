using GeometryWars.Entities;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Components.Visuals;

public interface IDrawableComponent
{
    void Draw(Entity owner, SpriteBatch spriteBatch);
}
