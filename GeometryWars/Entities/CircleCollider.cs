namespace GeometryWars;

// Data record struct describing an entity's collision shape.
// Detection is handled centrally by EntityManager (O(n²) circle-pair test).
// When two colliders overlap, EntityManager calls OnCollision(other) on both entities.
// Each entity defines its own response by overriding Entity.OnCollision().
//
// Refactored to a record struct to ensure data locality within the Entity class
// and to provide built-in value equality and better debugging (ToString).
public readonly record struct CircleCollider(float Radius)
{
    // A radius > 0 indicates this entity is collidable.
    public bool IsActive => Radius > 0;
}
