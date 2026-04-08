using System.Collections.Generic;
using System.Linq;
using GeometryWars.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

// Base class for all game entities.
public abstract class Entity
{
    private readonly List<IComponent> _components = [];

    public Texture2D Image { get; set; }
    public Color Tint { get; set; } = Color.White;
    public Vector2 Position { get; set; }
    public bool IsExpired { get; set; }

    // PERFORMANCE CACHE: 
    // Direct reference to the primary movement component.
    public MovementComponent Movement { get; private set; }

    public Vector2 Size => Image == null ? Vector2.Zero : new Vector2(Image.Width, Image.Height);

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
        
        // Internal cache for the manager/renderer
        if (component is MovementComponent mc)
            Movement = mc;

        // LIFECYCLE: Notify the component it has been attached so it can initialize.
        component.OnAdded(this);

        return component;
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        float orientation = Movement?.Orientation ?? 0f;
        spriteBatch.Draw(Image, Position, null, Tint, orientation, Size / 2f, 1f, 0, 0);

        foreach (var comp in _components)
            if (comp is IDrawableComponent dc)
                dc.Draw(this, spriteBatch);
    }
}
