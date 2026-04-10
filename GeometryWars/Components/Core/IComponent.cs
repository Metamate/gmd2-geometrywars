using GeometryWars.Entities;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Components.Core;

// Base contract for all entity capabilities.
public interface IComponent
{
    bool IsActive { get; set; }

    // Called immediately when the component is added to an entity.
    // Safe for self-setup only — siblings may not exist yet.
    void OnAdded(Entity owner);

    // Called once after the entity is fully assembled (all components added).
    // Safe for sibling lookups via owner.GetComponent<T>().
    void OnStart(Entity owner);

    void PreUpdate(Entity owner);
    void Update(Entity owner);
    void Simulate(Entity owner);
    void PostUpdate(Entity owner);
    void OnRemoved(Entity owner);
    void OnCollision(Entity owner, Entity other);
    void Draw(Entity owner, SpriteBatch spriteBatch);
}
