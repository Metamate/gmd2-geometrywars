using System.Collections.Generic;
using GeometryWars.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

// Base class for all game entities.
//
// Component system: AddComponent() registers an IComponent to run each frame.
// Every piece of logic (AI, Input, Physics) is a component. 
// Entities are simply containers for components and shared data (Position, Velocity).
public abstract class Entity
{
    private readonly List<IComponent> _components = [];

    public Texture2D Image { get; set; }
    public Color Tint { get; set; } = Color.White;

    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public float Orientation { get; set; }

    public bool IsExpired { get; set; }

    public CircleCollider Collider { get; protected set; } = new(0);

    public Vector2 Size => Image == null ? Vector2.Zero : new Vector2(Image.Width, Image.Height);

    public void Update()
    {
        // Pure Component Architecture: the entity has no logic of its own.
        // It simply delegates all work to its components.
        foreach (var comp in _components)
            comp.Update(this);
    }

    public virtual void OnCollision(Entity other) 
    {
        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i] is ICollisionComponent cc)
                cc.OnCollision(this, other);
        }
    }

    public T AddComponent<T>(T component) where T : IComponent
    {
        _components.Add(component);
        return component;
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Image, Position, null, Tint, Orientation, Size / 2f, 1f, 0, 0);

        foreach (var comp in _components)
            if (comp is IDrawableComponent dc)
                dc.Draw(this, spriteBatch);
    }
}
