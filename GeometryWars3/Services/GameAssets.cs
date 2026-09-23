using System;
using System.Linq;
using GeometryWars3.Definitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars3.Services;

// Holds all loaded content assets for the running game instance.
public sealed class GameAssets
{
    public Texture2D Player { get; private set; }
    public Texture2D Seeker { get; private set; }
    public Texture2D Wanderer { get; private set; }
    public Texture2D Bullet { get; private set; }
    public Texture2D BlackHole { get; private set; }
    public Texture2D LineParticle { get; private set; }
    public Texture2D Glow { get; private set; }
    public Texture2D Pixel { get; private set; }
    public Texture2D Pointer { get; private set; }
    public SpriteFont Font { get; private set; }

    public Texture2D GetTexture(SpriteId spriteId) => spriteId switch
    {
        SpriteId.Player => Player,
        SpriteId.Seeker => Seeker,
        SpriteId.Wanderer => Wanderer,
        SpriteId.Bullet => Bullet,
        SpriteId.BlackHole => BlackHole,
        SpriteId.LineParticle => LineParticle,
        SpriteId.Glow => Glow,
        SpriteId.Pointer => Pointer,
        _ => throw new ArgumentOutOfRangeException(nameof(spriteId), spriteId, "Unknown sprite id.")
    };

    public void Load(ContentManager content)
    {
        Player = content.Load<Texture2D>("Art/Player");
        Seeker = content.Load<Texture2D>("Art/Seeker");
        Wanderer = content.Load<Texture2D>("Art/Wanderer");
        Bullet = content.Load<Texture2D>("Art/Bullet");
        BlackHole = content.Load<Texture2D>("Art/BlackHole");
        Pointer = content.Load<Texture2D>("Art/Pointer");
        LineParticle = content.Load<Texture2D>("Art/Laser");
        Glow = content.Load<Texture2D>("Art/Glow");

        Pixel = new Texture2D(Player.GraphicsDevice, 1, 1);
        Pixel.SetData([Color.White]);

        Font = content.Load<SpriteFont>("font");
    }
}
