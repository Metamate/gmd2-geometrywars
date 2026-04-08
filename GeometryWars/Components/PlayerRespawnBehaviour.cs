using Microsoft.Xna.Framework;
using GeometryWars.Services;

namespace GeometryWars.Components;

public sealed class PlayerRespawnBehaviour : IComponent
{
    private int _framesUntilRespawn = 0;
    private TransformComponent _transform;

    public bool IsDead => _framesUntilRespawn > 0;

    public void Kill(int frames) => _framesUntilRespawn = frames;

    public void OnAdded(Entity owner)
    {
        _transform = owner.Transform;
    }

    public void Update(Entity owner)
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
