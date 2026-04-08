namespace GeometryWars.Components;

public interface IComponent
{
    bool IsActive { get; set; }

    void OnAdded(Entity owner);
    void Update(Entity owner);
}
