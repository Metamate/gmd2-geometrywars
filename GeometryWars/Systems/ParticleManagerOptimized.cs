using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

/// <summary>
/// ADVANCED: A high-performance particle manager using Data Oriented Design.
/// 
/// Educational Value (DOD & Data Locality):
/// 1. Value Memory: Particle is a struct. All particle data is stored "in-line"
///    within the array, meaning zero pointer chasing for the CPU.
/// 2. Contiguous Memory: The Particle[] array is a solid block of memory. 
///    CPUs can pre-fetch this data perfectly, maximizing Cache Hits.
/// 3. Zero Garbage: No objects are created or destroyed during the game loop.
/// 4. Sifting Swap: Deletion is O(1) and keeps the array packed by swapping the 
///    dead particle with the last living one.
/// </summary>
public class ParticleManagerOptimized<T> where T : struct
{
    // Custom delegate because 'ref' is not supported by standard Action<T>.
    public delegate void UpdateParticleDelegate(ref Particle particle);

    private readonly UpdateParticleDelegate _updateParticle;
    private readonly Particle[] _list;
    private int _count;
    private readonly int _capacity;

    public ParticleManagerOptimized(int capacity, UpdateParticleDelegate updateParticle)
    {
        _capacity = capacity;
        _updateParticle = updateParticle;
        _list = new Particle[capacity];
    }

    public void Update()
    {
        for (int i = 0; i < _count; i++)
        {
            // Use 'ref' to modify the struct directly in the array memory.
            ref var p = ref _list[i];
            
            _updateParticle(ref p);
            p.PercentLife -= 1f / p.Duration;

            if (p.PercentLife < 0)
            {
                // Sifting Swap: copy the last living particle into this slot.
                _list[i] = _list[_count - 1];
                _count--;
                i--; // Process the new particle we just moved into this slot.
            }
        }
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
            index = Random.Shared.Next(0, _capacity);
        }

        ref var p = ref _list[index];
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
            ref var p = ref _list[i];
            Vector2 origin = new(p.Texture.Width / 2f, p.Texture.Height / 2f);
            spriteBatch.Draw(p.Texture, p.Position, null, p.Tint, p.Orientation, origin, p.Scale, SpriteEffects.None, 0f);
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
