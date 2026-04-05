using System.Collections.Generic;
using GeometryWars.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

// Base class for all game entities.
//
// Uses the Template Method pattern: Update() defines a fixed execution order —
// OnUpdate() (entity logic) always runs before components (physics etc.) so that
// VelocityMover always sees the velocity the entity just wrote, not last frame's.
// Subclasses override OnUpdate() and must never override Update() directly.
public abstract class Entity
{
    private readonly List<IComponent> _components = [];

    protected Texture2D Image { get; set; }
    protected Color Tint { get; set; } = Color.White;

    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public float Orientation { get; set; }

    // IsExpired flags the entity for removal at the end of the current frame.
    // Entities are never removed mid-update to avoid corrupting the entity list
    // while EntityManager is iterating it.
    public bool IsExpired { get; set; }

    // Null means the entity has no collision shape and is skipped by EntityManager.
    public CircleCollider Collider { get; protected set; }

    public Vector2 Size => Image == null ? Vector2.Zero : new Vector2(Image.Width, Image.Height);

    public void Update()
    {
        OnUpdate();
        foreach (var comp in _components)
            comp.Update(this);
    }

    protected virtual void OnUpdate() { }

    protected T AddComponent<T>(T component) where T : IComponent
    {
        _components.Add(component);
        return component;
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Image, Position, null, Tint, Orientation, Size / 2f, 1f, 0, 0);
    }
}
