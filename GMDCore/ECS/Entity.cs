using System.Collections.Generic;
using System;
using GMDCore.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GMDCore.ECS;

// Base class for all game objects.
public class Entity
{
    private readonly List<IComponent> _components = [];

    public bool IsActive { get; set; } = true;
    public bool IsExpired { get; set; }

    public Transform Transform { get; private set; }
    public Vector2 Position => Transform.Position;

    public IReadOnlyList<IComponent> Components => _components;

    public Entity()
    {
        Transform = AddComponent(new Transform(Vector2.Zero));
    }

    public T GetComponent<T>() where T : class, IComponent
    {
        for (int i = 0; i < _components.Count; i++)
            if (_components[i] is T t) return t;
        return null;
    }

    public bool HasComponent<T>() where T : class, IComponent
        => GetComponent<T>() != null;

    public T RequireComponent<T>() where T : class, IComponent
        => GetComponent<T>() ?? throw new InvalidOperationException($"{GetType().Name} is missing required component {typeof(T).Name}.");

    public void Update()
    {
        if (!IsActive) return;

        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i].IsActive)
                _components[i].PreUpdate(this);
        }

        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i].IsActive)
                _components[i].Update(this);
        }

        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i].IsActive)
                _components[i].Simulate(this);
        }

        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i].IsActive)
                _components[i].PostUpdate(this);
        }
    }

    // Broadcasts collision events to all active components.
    public virtual void OnCollision(Entity other)
    {
        if (!IsActive) return;

        for (int i = 0; i < _components.Count; i++)
            if (_components[i].IsActive)
                _components[i].OnCollision(this, other);
    }

    public T AddComponent<T>(T component) where T : IComponent
    {
        _components.Add(component);
        if (component is Transform tc) Transform = tc;
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

    public void Remove()
    {
        for (int i = 0; i < _components.Count; i++)
            _components[i].OnRemoved(this);
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (!IsActive) return;

        for (int i = 0; i < _components.Count; i++)
            if (_components[i].IsActive)
                _components[i].Draw(this, spriteBatch);
    }
}

