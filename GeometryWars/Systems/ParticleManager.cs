using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

// DATA LOCALITY NOTE — Array of Objects (current layout):
// CircularParticleArray holds Particle references. The Particle objects themselves
// live scattered across the heap, so iterating the array causes one pointer
// dereference per particle — each likely a cache miss at high particle counts.
//
// A more cache-friendly layout (Struct of Arrays / SoA) would store each field
// in its own contiguous array:
//     float[] positionX, positionY;
//     float[] orientations;
//     Color[] tints;
//     ...
// This lets the CPU prefetch and process fields in tight loops without chasing
// pointers. ParticleState is already a value type (struct), which is a step in
// that direction — note how it is stored inline in Particle.State rather than
// as a separate heap object.
public class ParticleManager<T>
{
    private readonly Action<Particle> updateParticle;
    private readonly CircularParticleArray particleList;
    public ParticleManager(int capacity, Action<Particle> updateParticle)
    {
        this.updateParticle = updateParticle;
        particleList = new CircularParticleArray(capacity);
        // Populate the list with empty particle objects, for reuse. 
        for (int i = 0; i < capacity; i++)
            particleList[i] = new Particle();
    }

    public void Update()
    {
        int removalCount = 0;
        for (int i = 0; i < particleList.Count; i++)
        {
            var particle = particleList[i];
            updateParticle(particle);
            particle.PercentLife -= 1f / particle.Duration;
            // sift deleted particles to the end of the list
            Swap(particleList, i - removalCount, i);
            if (particle.PercentLife < 0)
                removalCount++;
        }
        particleList.Count -= removalCount;
    }
    private static void Swap(CircularParticleArray list, int index1, int index2)
    {
        (list[index2], list[index1]) = (list[index1], list[index2]);
    }

    public void CreateParticle(Texture2D texture, Vector2 position, Color tint, float duration, float scale, T state, float theta = 0)
    {
        CreateParticle(texture, position, tint, duration, new Vector2(scale), state, theta);
    }

    public void CreateParticle(Texture2D texture, Vector2 position, Color tint, float duration, Vector2 scale, T state, float theta = 0)
    {
        Particle particle;
        if (particleList.Count == particleList.Capacity)
        {
            // if the list is full, overwrite the oldest particle, and rotate the circular list 
            particle = particleList[0];
            particleList.Start++;
        }
        else
        {
            particle = particleList[particleList.Count];
            particleList.Count++;
        }
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
        for (int i = 0; i < particleList.Count; i++)
        {
            var particle = particleList[i];
            Vector2 origin = new(particle.Texture.Width / 2, particle.Texture.Height / 2);
            spriteBatch.Draw(particle.Texture, particle.Position, null, particle.Tint, particle.Orientation, origin, particle.Scale, 0, 0);
        }
    }

    public class Particle
    {
        public Texture2D Texture;
        public Vector2 Position;
        public float Orientation;
        public Vector2 Scale = Vector2.One;
        public Color Tint;
        public float Duration;
        public float PercentLife = 1f;
        public T State;
    }

    private class CircularParticleArray(int capacity)
    {
        private int start;
        public int Start
        {
            get { return start; }
            set { start = value % list.Length; }
        }
        public int Count { get; set; }
        public int Capacity { get { return list.Length; } }
        private readonly Particle[] list = new Particle[capacity];

        public Particle this[int i]
        {
            get { return list[(start + i) % list.Length]; }
            set { list[(start + i) % list.Length] = value; }
        }
    }
}