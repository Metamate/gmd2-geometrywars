using Microsoft.Xna.Framework;

namespace GeometryWars.Systems;

public interface IGridField
{
    void ApplyDirectedForce(Vector2 force, Vector2 position, float radius);
    void ApplyImplosiveForce(float force, Vector2 position, float radius);
    void ApplyExplosiveForce(float force, Vector2 position, float radius);

    // Pushes grid points along the Z axis, creating a depth "punch" effect.
    void ApplyDepthPulse(Vector2 position, float force, float radius);
}
