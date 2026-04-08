using Microsoft.Xna.Framework;

namespace GeometryWars.Systems;

public interface IGridField
{
    void ApplyDirectedForce(Vector2 force, Vector2 position, float radius);
    void ApplyDirectedForce(Vector3 force, Vector3 position, float radius);
    void ApplyImplosiveForce(float force, Vector2 position, float radius);
    void ApplyImplosiveForce(float force, Vector3 position, float radius);
    void ApplyExplosiveForce(float force, Vector2 position, float radius);
    void ApplyExplosiveForce(float force, Vector3 position, float radius);
}
