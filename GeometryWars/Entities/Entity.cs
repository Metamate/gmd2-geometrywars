using System.Collections.Generic;
using System.Linq;
using GeometryWars.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

// Base class for all game entities.
//
// PURE COMPONENT ARCHITECTURE:
// The entity has zero built-in logic or spatial data. 
// It is simply a container for a list of Components.
public abstract class Entity
{
    private readonly List<IComponent> _components = [];

    public bool IsExpired { get; set; }

    // PERFORMANCE CACHE: Direct references to common components.
    public TransformComponent Transform { get; private set; }
    public MovementComponent Movement { get; private set; }

    // Convenience helpers
    public Vector2 Position => Transform?.Position ?? Vector2.Zero;

    public T GetComponent<T>() where T : class, IComponent 
        => _components.OfType<T>().FirstOrDefault();

    public void Update()
    {
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
        
        if (component is TransformComponent tc) Transform = tc;
        else if (component is MovementComponent mc) Movement = mc;

        component.OnAdded(this);
        return component;
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        foreach (var comp in _components)
            if (comp is IDrawableComponent dc)
                dc.Draw(this, spriteBatch);
    }
}
