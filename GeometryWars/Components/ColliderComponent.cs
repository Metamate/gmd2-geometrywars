namespace GeometryWars.Components;

public abstract class ColliderComponent : IComponent
{
    public bool IsActive { get; set; } = true;

    public virtual void OnAdded(Entity owner) { }
    public virtual void Update(Entity owner) { }
}
