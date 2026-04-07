using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

// Manages a list of game particles.
//
// Removal Logic (Sifting):
// When a particle dies, we swap it with the LAST living particle in the array.
// This is a simple, high-performance way to keep the list contiguous without
// needing complex circular buffers or moving every item in memory.
//
// Note: See ParticleManagerOptimized.cs for a high-performance version of 
// this system using Data Oriented Design (DOD) and Memory Locality.
public class ParticleManager<T>
{
    private readonly Action<Particle> _updateParticle;
    private readonly Particle[] _list;
    private int _count;
    private readonly int _capacity;

    public ParticleManager(int capacity, Action<Particle> updateParticle)
    {
        _capacity = capacity;
        _updateParticle = updateParticle;
        _list = new Particle[capacity];

        // Pre-populate the array with objects we can reuse.
        for (int i = 0; i < capacity; i++)
            _list[i] = new Particle();
    }

    public void Update()
    {
        for (int i = 0; i < _count; i++)
        {
            Particle p = _list[i];
            _updateParticle(p);
            p.PercentLife -= 1f / p.Duration;

            // If dead, swap with the last living particle and decrease count.
            if (p.PercentLife < 0)
            {
                // Note: We don't actually delete the object, we just swap
                // its position in the array so it's outside the "living" range.
                Particle last = _list[_count - 1];
                _list[i] = last;
                _list[_count - 1] = p;

                _count--;
                i--; // Process the particle we just swapped into this slot.
            }
        }
    }

    public void CreateParticle(Texture2D texture, Vector2 position, Color tint, float duration, float scale, T state, float theta = 0)
    {
        CreateParticle(texture, position, tint, duration, new Vector2(scale), state, theta);
    }

    public void CreateParticle(Texture2D texture, Vector2 position, Color tint, float duration, Vector2 scale, T state, float theta = 0)
    {
        Particle p;
        if (_count < _capacity)
        {
            p = _list[_count];
            _count++;
        }
        else
        {
            // If the buffer is full, overwrite a random living particle.
            p = _list[Random.Shared.Next(0, _capacity)];
        }

        p.Texture = texture;
        p.Position = position;
        p.Tint = tint;
        p.Duration = duration;
        p.PercentLife = 1f;
        p.Scale = scale;
        p.Orientation = theta;
        p.State = state;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < _count; i++)
        {
            Particle p = _list[i];
            Vector2 origin = new(p.Texture.Width / 2f, p.Texture.Height / 2f);
            spriteBatch.Draw(p.Texture, p.Position, null, p.Tint, p.Orientation, origin, p.Scale, SpriteEffects.None, 0f);
        }
    }

    public class Particle
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
