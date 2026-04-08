using System.Collections.Generic;
using System.Linq;
using GeometryWars.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

// Base class for all game entities.
//
// FINAL FORM: PURE COMPONENT ARCHITECTURE
// The entity has zero built-in logic or spatial data. 
// It is simply a container for a list of Components.
public abstract class Entity
{
    private readonly List<IComponent> _components = [];

    public bool IsExpired { get; set; }

    // PERFORMANCE CACHE: Direct references to common components.
    public TransformComponent Transform { get; private set; }

    // Convenience helper
    public Vector2 Position => Transform?.Position ?? Vector2.Zero;

    public T GetComponent<T>() where T : class, IComponent 
        => _components.OfType<T>().FirstOrDefault();

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
        
        // Cache spatial/physics components
        if (component is TransformComponent tc) Transform = tc;

        component.OnAdded(this);
        return component;
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        // asks its components (like SpriteComponent) to do it.
        foreach (var comp in _components)
            if (comp is IDrawableComponent dc)
                dc.Draw(this, spriteBatch);
    }
}
