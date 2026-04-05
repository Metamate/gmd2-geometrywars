using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

static class Art
{
    public static Texture2D Player { get; set; }
    public static Texture2D Seeker { get; set; }
    public static Texture2D Wanderer { get; set; }
    public static Texture2D Bullet { get; set; }
    public static Texture2D Pointer { get; set; }
    public static Texture2D BlackHole { get; set; }
    public static Texture2D LineParticle { get; set; }
    public static Texture2D Glow { get; set; }
    public static Texture2D Pixel { get; private set; }
    public static SpriteFont Font { get; set; }

    public static void Load(ContentManager content)
    {
        Player = content.Load<Texture2D>("Art/Player");
        Seeker = content.Load<Texture2D>("Art/Seeker");
        Wanderer = content.Load<Texture2D>("Art/Wanderer");
        Bullet = content.Load<Texture2D>("Art/Bullet");
        Pointer = content.Load<Texture2D>("Art/Pointer");
        BlackHole = content.Load<Texture2D>("Art/BlackHole");
        LineParticle = content.Load<Texture2D>("Art/Laser");
        Glow = content.Load<Texture2D>("Art/Glow");
        Pixel = new Texture2D(Player.GraphicsDevice, 1, 1);
        Pixel.SetData([Color.White]);

        Font = content.Load<SpriteFont>("font");
    }

}