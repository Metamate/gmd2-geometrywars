using Microsoft.Xna.Framework.Audio;

namespace GeometryWars.Services;

/// <summary>
/// A simple service for playing sounds.
/// Provides a central point to control volume or mute the game.
/// </summary>
public sealed class AudioManager
{
    public float Volume { get; set; } = 1.0f;

    public void Play(SoundEffect sound, float volume = 1.0f, float pitch = 0f, float pan = 0f)
    {
        sound.Play(volume * Volume, pitch, pan);
    }
}
