using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GMDCore.Particles;

// Runtime storage for one live particle inside the particle manager.
public sealed class ParticleInstance<T>
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

