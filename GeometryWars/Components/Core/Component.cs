using GeometryWars.Entities;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Components.Core;

// Provides default activation and lifecycle hooks for all components.
public abstract class Component : IComponent
{
    public bool IsActive { get; set; } = true;
    public virtual void OnAdded(Entity owner) { }
    public virtual void OnStart(Entity owner) { }
    public virtual void PreUpdate(Entity owner) { }
    public virtual void Update(Entity owner) { }
    public virtual void Simulate(Entity owner) { }
    public virtual void PostUpdate(Entity owner) { }
    public virtual void OnRemoved(Entity owner) { }
    public virtual void OnCollision(Entity owner, Entity other) { }
    public virtual void Draw(Entity owner, SpriteBatch spriteBatch) { }
}
