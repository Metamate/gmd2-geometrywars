using System;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace GeometryWars;

static class Sound
{
    public static Song Music { get; private set; }
    private static SoundEffect[] explosions;
    public static SoundEffect Explosion => explosions[Random.Shared.Next(explosions.Length)];
    private static SoundEffect[] shots;
    public static SoundEffect Shot => shots[Random.Shared.Next(shots.Length)];
    private static SoundEffect[] spawns;
    public static SoundEffect Spawn => spawns[Random.Shared.Next(spawns.Length)];
    public static void Load(ContentManager content)
    {
        MediaPlayer.Volume = 0.5f;
        SoundEffect.MasterVolume = 1.0f;
        Music = content.Load<Song>("Audio/Music");
        // Enumerable.Range + Select loads all numbered sound files into an array concisely.
        // e.g. "Audio/explosion-01" through "Audio/explosion-08".
        explosions = [.. Enumerable.Range(1, 8).Select(x => content.Load<SoundEffect>("Audio/explosion-0" + x))];
        shots = [.. Enumerable.Range(1, 4).Select(x => content.Load<SoundEffect>("Audio/shoot-0" + x))];
        spawns = [.. Enumerable.Range(1, 8).Select(x => content.Load<SoundEffect>("Audio/spawn-0" + x))];
    }
}