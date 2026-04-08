using System.Collections.Generic;
using System.Linq;
using GeometryWars.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

public abstract class Entity
{
    private readonly List<IComponent> _components = [];

    public bool IsExpired { get; set; }

    public TransformComponent Transform { get; private set; }

    public bool IsActive => Transform.IsActive;

    public Vector2 Position => Transform.Position;

    protected Entity()
    {
        Transform = AddComponent(new TransformComponent(Vector2.Zero));
    }

    public T GetComponent<T>() where T : class, IComponent 
        => _components.OfType<T>().FirstOrDefault();

    public void Update()
    {
        // PERFORMANCE: If the entity is dormant (e.g. still spawning), we still need
        // to update its "Lifecycle" components, but we skip everything else.
        // We iterate through all components because lifecycle components like 
        // SpawnFadeBehaviour manage the IsActive state from the inside.
        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i].IsActive)
                _components[i].Update(this);
        }
    }

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
