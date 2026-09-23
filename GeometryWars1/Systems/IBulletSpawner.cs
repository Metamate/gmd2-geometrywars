using Microsoft.Xna.Framework;

namespace GeometryWars1.Systems;

public interface IBulletSpawner
{
    void SpawnBullet(Vector2 position, Vector2 velocity);
}
