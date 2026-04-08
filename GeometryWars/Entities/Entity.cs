using System.Collections.Generic;
using System.Linq;
using GeometryWars.Components.Core;
using GeometryWars.Components.Combat;
using GeometryWars.Components.Visuals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Entities;

// Base class for all game objects.
public abstract class Entity
{
    private readonly List<IComponent> _components = [];

    public bool IsActive { get; set; } = true;
    public bool IsExpired { get; set; }

    // Direct access to spatial identity.
    public TransformComponent Transform { get; private set; }
    public Vector2 Position => Transform.Position;

    public IReadOnlyList<IComponent> Components => _components;

    protected Entity()
    {
        Transform = AddComponent(new TransformComponent(Vector2.Zero));
    }

    public T GetComponent<T>() where T : class, IComponent 
        => _components.OfType<T>().FirstOrDefault();

    public void Update()
    {
        if (!IsActive) return;

        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i].IsActive)
                _components[i].Update(this);
        }
    }

    // Broadcasts collision events to all active components.
    public virtual void OnCollision(Entity other) 
    {
        if (!IsActive) return;

        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i].IsActive && _components[i] is ICollisionComponent cc)
                cc.OnCollision(this, other);
        }
    }

    public T AddComponent<T>(T component) where T : IComponent
    {
        _components.Add(component);
        if (component is TransformComponent tc) Transform = tc;
        component.OnAdded(this);
        return component;
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (!IsActive) return;

        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i].IsActive && _components[i] is IDrawableComponent dc)
                dc.Draw(this, spriteBatch);
        }
    }

    public void SetAllComponentsActive(bool active)
    {
        foreach (var comp in _components)
            comp.IsActive = active;
    }
}
