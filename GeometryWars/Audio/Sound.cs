using System;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace GeometryWars;

// Holds all loaded audio assets.
// Each sound category is an array; the property picks a random variant on every access,
// giving natural variation without any call-site logic.
static class Sound
{
    public static Song Music { get; private set; }

    private static SoundEffect[] _explosions;
    private static SoundEffect[] _shots;
    private static SoundEffect[] _spawns;

    public static SoundEffect Explosion => _explosions[Random.Shared.Next(_explosions.Length)];
    public static SoundEffect Shot      => _shots[Random.Shared.Next(_shots.Length)];
    public static SoundEffect Spawn     => _spawns[Random.Shared.Next(_spawns.Length)];

    public static void Load(ContentManager content)
    {
        Music      = content.Load<Song>("Audio/Music");
        _explosions = [.. Enumerable.Range(1, 8).Select(i => content.Load<SoundEffect>($"Audio/explosion-{i:D2}"))];
        _shots      = [.. Enumerable.Range(1, 4).Select(i => content.Load<SoundEffect>($"Audio/shoot-{i:D2}"))];
        _spawns     = [.. Enumerable.Range(1, 8).Select(i => content.Load<SoundEffect>($"Audio/spawn-{i:D2}"))];
    }
}
