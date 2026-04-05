using System;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace GeometryWars;

static class Sound
{
    public static Song Music { get; private set; }
    private static readonly Random rand = new();
    private static SoundEffect[] explosions;
    // return a random explosion sound 
    public static SoundEffect Explosion { get { return explosions[rand.Next(explosions.Length)]; } }
    private static SoundEffect[] shots;
    public static SoundEffect Shot { get { return shots[rand.Next(shots.Length)]; } }
    private static SoundEffect[] spawns;
    public static SoundEffect Spawn { get { return spawns[rand.Next(spawns.Length)]; } }
    public static void Load(ContentManager content)
    {
        MediaPlayer.Volume = 0.00f;
        SoundEffect.MasterVolume = 0.00f;
        Music = content.Load<Song>("Audio/Music");
        // These linq expressions are just a fancy way loading all sounds of each category into an array. 
        explosions = [.. Enumerable.Range(1, 8).Select(x => content.Load<SoundEffect>("Audio/explosion-0" + x))];
        shots = [.. Enumerable.Range(1, 4).Select(x => content.Load<SoundEffect>("Audio/shoot-0" + x))];
        spawns = [.. Enumerable.Range(1, 8).Select(x => content.Load<SoundEffect>("Audio/spawn-0" + x))];
    }
}