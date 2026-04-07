using System.Collections.Generic;
using GeometryWars.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

// Base class for all game entities.
//
// Component system: AddComponent() registers an IComponent to run each frame.
// Components that also implement IDrawableComponent are called during Draw().
// Components decouple reusable behaviours (physics, wrapping, lifetime) from the
// entity class hierarchy, so they can be mixed and matched without subclassing.
//
// Update order: OnUpdate() (entity logic) always runs before component updates
// so that VelocityMover always sees the velocity the entity wrote this frame,
// not last frame's. Subclasses override OnUpdate() and must never override Update().
//
// Collision: EntityManager calls OnCollision(other) on both entities when their
// circles overlap. Override OnCollision() in a subclass to define the response.
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

    // Struct-based collider. If Collider.IsActive is false (Radius <= 0),
    // the entity is skipped by EntityManager's collision pass.
    public CircleCollider Collider { get; protected set; } = new(0);

    public Vector2 Size => Image == null ? Vector2.Zero : new Vector2(Image.Width, Image.Height);

    public void Update()
    {
        OnUpdate();
        foreach (var comp in _components)
            comp.Update(this);
    }

    protected virtual void OnUpdate() { }

    // Called by EntityManager when this entity's collider overlaps another entity's.
    // Override in subclasses to define the collision response: take damage, expire,
    // bounce, score points, etc. The base implementation is intentionally empty —
    // entities that do not care about a particular collision type can ignore it silently.
    public virtual void OnCollision(Entity other) { }

    protected T AddComponent<T>(T component) where T : IComponent
    {
        _components.Add(component);
        return component;
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Image, Position, null, Tint, Orientation, Size / 2f, 1f, 0, 0);

        // Drawable components render on top of the entity's own sprite.
        // This lets components add overlays (glows, shields, debug shapes)
        // without the entity knowing anything about them.
        foreach (var comp in _components)
            if (comp is IDrawableComponent dc)
                dc.Draw(this, spriteBatch);
    }
}
