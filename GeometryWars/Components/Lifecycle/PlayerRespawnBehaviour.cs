using GeometryWars.Components.Core;
using GeometryWars.Entities;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.Lifecycle;

/// <summary>
/// Handles the player ship's death timer and respawn effects.
/// </summary>
public sealed class PlayerRespawnBehaviour : Component
{
    private int _framesUntilRespawn = 0;
    private TransformComponent _transform;

    public bool IsDead => _framesUntilRespawn > 0;

    public void Kill(int frames) => _framesUntilRespawn = frames;

    public override void OnAdded(Entity owner)
    {
        _transform = owner.Transform;
    }

    public override void Update(Entity owner)
    {
        if (IsDead)
        {
            _framesUntilRespawn--;
            
            if (_framesUntilRespawn == 0 && !PlayerStatus.IsGameOver && _transform != null)
            {
                GameServices.Grid.ApplyDirectedForce(new Vector3(0, 0, 5000), new Vector3(_transform.Position, 0), 50);
            }
        }
    }
}
