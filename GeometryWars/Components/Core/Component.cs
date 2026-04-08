using GeometryWars.Entities;

namespace GeometryWars.Components.Core;

public abstract class Component : IComponent
{
    public bool IsActive { get; set; } = true;
    public virtual void OnAdded(Entity owner) { }
    public abstract void Update(Entity owner);
}
