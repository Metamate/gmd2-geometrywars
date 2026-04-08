namespace GeometryWars.Components;

public interface IComponent
{
    // Called once when the component is attached to an entity.
    // Ideal for caching references to sibling components.
    void OnAdded(Entity owner);

    void Update(Entity owner);
}
