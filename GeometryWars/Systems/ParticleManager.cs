using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Systems;

public class ParticleManager<T> : IParticleSystem<T>
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

            if (p.PercentLife < 0)
            {
                Particle last = _list[_count - 1];
                _list[i] = last;
                _list[_count - 1] = p;

                _count--;
                i--; 
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
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public float Orientation { get; set; }
        public Vector2 Scale { get; set; }
        public Color Tint { get; set; }
        public float Duration { get; set; }
        public float PercentLife { get; set; }
        public T State { get; set; }
    }
}
