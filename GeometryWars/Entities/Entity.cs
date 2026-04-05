using System.Collections.Generic;
using GeometryWars.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

public abstract class Entity
{
    protected Texture2D image;
    protected Color color = Color.White;

    private readonly List<IComponent> _components = [];

    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public float Orientation { get; set; }
    public bool IsExpired { get; set; }
    public CircleCollider Collider { get; private set; }

    public Vector2 Size => image == null ? Vector2.Zero : new Vector2(image.Width, image.Height);

    // Update order: OnPreUpdate (AI / input), then components (physics, etc.)
    public void Update()
    {
        OnPreUpdate();
        foreach (var comp in _components)
            comp.Update(this);
    }

    // Override for entity-specific logic that runs before components (AI, input, coroutines).
    protected virtual void OnPreUpdate() { }

    protected T AddComponent<T>(T component) where T : IComponent
    {
        _components.Add(component);
        if (component is CircleCollider c)
            Collider = c;
        return component;
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(image, Position, null, color, Orientation, Size / 2f, 1f, 0, 0);
    }
}
