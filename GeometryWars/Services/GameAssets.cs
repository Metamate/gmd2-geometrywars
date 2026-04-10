using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace GeometryWars.Services;

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

    public Song Music { get; private set; }

    private SoundEffect[] _explosions = Array.Empty<SoundEffect>();
    private SoundEffect[] _shots = Array.Empty<SoundEffect>();
    private SoundEffect[] _spawns = Array.Empty<SoundEffect>();

    public SoundEffect GetRandomExplosion() => _explosions[Random.Shared.Next(_explosions.Length)];
    public SoundEffect GetRandomShot() => _shots[Random.Shared.Next(_shots.Length)];
    public SoundEffect GetRandomSpawn() => _spawns[Random.Shared.Next(_spawns.Length)];

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

        Music = content.Load<Song>("Audio/Music");
        _explosions = [.. Enumerable.Range(1, 8).Select(i => content.Load<SoundEffect>($"Audio/explosion-{i:D2}"))];
        _shots = [.. Enumerable.Range(1, 4).Select(i => content.Load<SoundEffect>($"Audio/shoot-{i:D2}"))];
        _spawns = [.. Enumerable.Range(1, 8).Select(i => content.Load<SoundEffect>($"Audio/spawn-{i:D2}"))];
    }
}
