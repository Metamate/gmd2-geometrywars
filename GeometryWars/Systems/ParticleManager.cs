using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

// Manages high-performance particles using Data Oriented Design (DOD).
//
// Refactored for Data Locality:
// 1. Particle is now a struct.
// 2. All particles are stored in a flat array (contiguous memory).
// 3. Update/Draw loops use 'ref' to avoid copying structs.
public class ParticleManager<T> where T : struct
{
    public delegate void UpdateParticleDelegate(ref Particle particle);

    private readonly UpdateParticleDelegate _updateParticle;
    private readonly Particle[] _list;
    private int _count;
    private readonly int _capacity;

    public ParticleManager(int capacity, UpdateParticleDelegate updateParticle)
    {
        _capacity = capacity;
        _updateParticle = updateParticle;
        _list = new Particle[capacity];
    }

    public void Update()
    {
        for (int i = 0; i < _count; i++)
        {
            // Logic: Process every living particle.
            ref var particle = ref _list[i];

            _updateParticle(ref particle);
            particle.PercentLife -= 1f / particle.Duration;

            // Removal: If a particle dies, swap it with the LAST living particle
            // and decrease the count. This keeps the array contiguous without
            // needing complex circular logic or "ghost" particles.
            if (particle.PercentLife < 0)
            {
                _list[i] = _list[_count - 1];
                _count--;
                i--; // Re-process the particle we just swapped into this slot.
            }
        }
    }

    public void CreateParticle(Texture2D texture, Vector2 position, Color tint, float duration, float scale, T state, float theta = 0)
    {
        CreateParticle(texture, position, tint, duration, new Vector2(scale), state, theta);
    }

    public void CreateParticle(Texture2D texture, Vector2 position, Color tint, float duration, Vector2 scale, T state, float theta = 0)
    {
        int index;
        if (_count < _capacity)
        {
            index = _count;
            _count++;
        }
        else
        {
            // If full, overwrite a random particle to avoid "ghosting" bias
            // and keep the system responsive under load.
            index = Random.Shared.Next(0, _capacity);
        }

        ref var particle = ref _list[index];
        particle.Texture = texture;
        particle.Position = position;
        particle.Tint = tint;
        particle.Duration = duration;
        particle.PercentLife = 1f;
        particle.Scale = scale;
        particle.Orientation = theta;
        particle.State = state;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < _count; i++)
        {
            ref var particle = ref _list[i];
            Vector2 origin = new(particle.Texture.Width / 2f, particle.Texture.Height / 2f);
            spriteBatch.Draw(particle.Texture, particle.Position, null, particle.Tint, particle.Orientation, origin, particle.Scale, SpriteEffects.None, 0f);
        }
    }

    public struct Particle
    {
        public Texture2D Texture;
        public Vector2 Position;
        public float Orientation;
        public Vector2 Scale;
        public Color Tint;
        public float Duration;
        public float PercentLife;
        public T State;
    }
}
