namespace GeometryWars.Components;

public sealed class BulletCollisionBehaviour : ICollisionComponent
{
    public void OnAdded(Entity owner) { }

    public void Update(Entity owner) { }

    public void OnCollision(Entity owner, Entity other)
    {
        if (other is Enemy || other is BlackHole)
        {
            owner.IsExpired = true;
        }
    }
}
