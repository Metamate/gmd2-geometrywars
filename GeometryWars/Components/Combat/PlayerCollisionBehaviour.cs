using GeometryWars.Components.Core;
using GeometryWars.Entities;
using GeometryWars.Systems;

namespace GeometryWars.Components.Combat;

// Handles collision response for the player ship.
// Follows the same pattern as EnemyCollisionBehaviour and BulletCollisionBehaviour.
public sealed class PlayerCollisionBehaviour : Component, ICollisionComponent
{
    public override void Update(Entity owner) { }

    public void OnCollision(Entity owner, Entity other)
    {
        if (owner is not PlayerShip player || player.IsDead) return;

        bool hitByEnemy   = other is Enemy e && e.IsActive;
        bool hitByBlackHole = other is BlackHole;

        if (hitByEnemy || hitByBlackHole)
        {
            player.Kill();
            EntityManager.KillAllEnemies();
            if (hitByBlackHole) EntityManager.KillAllBlackHoles();
            EnemySpawner.Reset();
        }
    }
}
