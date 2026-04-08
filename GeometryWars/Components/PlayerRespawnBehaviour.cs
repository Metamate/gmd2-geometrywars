using Microsoft.Xna.Framework;
using GeometryWars.Services;

namespace GeometryWars.Components;

/// <summary>
/// Handles the player's death timer and respawn visual effects.
/// Demonstrates moving even "lifecycle" logic into components.
/// </summary>
public sealed class PlayerRespawnBehaviour : IComponent
{
    private int _framesUntilRespawn = 0;
    public bool IsDead => _framesUntilRespawn > 0;

    public void Kill(int frames) => _framesUntilRespawn = frames;

    public void Update(Entity owner)
    {
        if (owner is not PlayerShip player) return;

        if (IsDead)
        {
            _framesUntilRespawn--;
            
            // If the timer just hit zero, create a grid "pop" effect at the player's position
            if (_framesUntilRespawn == 0 && !PlayerStatus.IsGameOver)
            {
                GameServices.Grid.ApplyDirectedForce(new Vector3(0, 0, 5000), new Vector3(player.Position, 0), 50);
            }
        }
    }
}
