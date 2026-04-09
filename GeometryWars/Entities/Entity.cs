using System.Collections.Generic;
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
    private static readonly ComponentUpdatePhase[] UpdatePhases =
    [
        ComponentUpdatePhase.PreUpdate,
        ComponentUpdatePhase.Logic,
        ComponentUpdatePhase.Physics,
        ComponentUpdatePhase.PostPhysics,
    ];

    public bool IsActive { get; set; } = true;
    public bool IsExpired { get; set; }

    public TransformComponent Transform { get; private set; }
    public Vector2 Position => Transform.Position;

    public IReadOnlyList<IComponent> Components => _components;

    protected Entity()
    {
        Transform = AddComponent(new TransformComponent(Vector2.Zero));
    }

    public T GetComponent<T>() where T : class, IComponent
    {
        for (int i = 0; i < _components.Count; i++)
            if (_components[i] is T t) return t;
        return null;
    }

    public void Update()
    {
        if (!IsActive) return;

        for (int phaseIndex = 0; phaseIndex < UpdatePhases.Length; phaseIndex++)
        {
            var phase = UpdatePhases[phaseIndex];
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i].IsActive && _components[i].Phase == phase)
                    _components[i].Update(this);
            }
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

    // Call once after all components have been added to wire up sibling references.
    // Invoked by EntityWorld when the entity enters the current play session.
    public void Start()
    {
        for (int i = 0; i < _components.Count; i++)
            _components[i].OnStart(this);
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
}
