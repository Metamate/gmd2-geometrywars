namespace GeometryWars.Components;

public sealed class BulletCollisionBehaviour : Component, ICollisionComponent
{
    public override void Update(Entity owner) { }

    public void OnCollision(Entity owner, Entity other)
    {
        if (other is Enemy || other is BlackHole)
        {
            owner.IsExpired = true;
        }
    }
}
