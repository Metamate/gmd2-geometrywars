using Microsoft.Xna.Framework;

namespace GeometryWars3.Systems;

public interface IBulletSpawner
{
    void SpawnBullet(Vector2 position, Vector2 velocity);
}
