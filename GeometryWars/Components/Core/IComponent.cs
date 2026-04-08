using GeometryWars.Entities;

namespace GeometryWars.Components.Core;

// Base contract for all entity capabilities.
public interface IComponent
{
    bool IsActive { get; set; }
    void OnAdded(Entity owner);
    void Update(Entity owner);
}
