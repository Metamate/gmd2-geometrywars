using Microsoft.Xna.Framework;

namespace GeometryWars6.Systems;

public interface IBulletSpawner
{
    void SpawnBullet(Vector2 position, Vector2 velocity);
}
