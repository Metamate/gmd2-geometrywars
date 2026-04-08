using Microsoft.Xna.Framework;
using GeometryWars.Services;

namespace GeometryWars.Components;

public sealed class PlayerRespawnBehaviour : IComponent
{
    private int _framesUntilRespawn = 0;
    public bool IsDead => _framesUntilRespawn > 0;

    public void Kill(int frames) => _framesUntilRespawn = frames;

    public void OnAdded(Entity owner) { }

    public void Update(Entity owner)
    {
        if (owner is not PlayerShip player) return;

        if (IsDead)
        {
            _framesUntilRespawn--;
            
            if (_framesUntilRespawn == 0 && !PlayerStatus.IsGameOver)
            {
                GameServices.Grid.ApplyDirectedForce(new Vector3(0, 0, 5000), new Vector3(player.Position, 0), 50);
            }
        }
    }
}
