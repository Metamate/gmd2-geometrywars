using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

public abstract class Entity
{
    protected Texture2D image;
    protected Color color = Color.White;

    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public float Orientation { get; set; }
    public float Radius { get; set; } = 20;
    public bool IsExpired { get; set; }

    public Vector2 Size => image == null ? Vector2.Zero : new Vector2(image.Width, image.Height);

    public abstract void Update(GameContext ctx);

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(image, Position, null, color, Orientation, Size / 2f, 1f, 0, 0);
    }
}
