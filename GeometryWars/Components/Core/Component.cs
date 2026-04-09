using GeometryWars.Entities;

namespace GeometryWars.Components.Core;

// Provides default activation and lifecycle hooks for all components.
public abstract class Component : IComponent
{
    public bool IsActive { get; set; } = true;
    public virtual ComponentUpdatePhase Phase => ComponentUpdatePhase.Logic;
    public virtual void OnAdded(Entity owner) { }
    public virtual void OnStart(Entity owner) { }
    public abstract void Update(Entity owner);
}
