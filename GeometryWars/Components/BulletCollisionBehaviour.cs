namespace GeometryWars.Components;

/// <summary>
/// Handles collision logic specifically for Bullets.
/// </summary>
public sealed class BulletCollisionBehaviour : ICollisionComponent
{
    public void Update(Entity owner) { }

    public void OnCollision(Entity owner, Entity other)
    {
        // Bullets expire on contact with an enemy or black hole.
        if (other is Enemy || other is BlackHole)
        {
            owner.IsExpired = true;
        }
    }
}
