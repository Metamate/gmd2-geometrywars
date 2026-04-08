namespace GeometryWars.Components;

public abstract class Component : IComponent
{
    public bool IsActive { get; set; } = true;

    public virtual void OnAdded(Entity owner) { }

    public abstract void Update(Entity owner);
}
