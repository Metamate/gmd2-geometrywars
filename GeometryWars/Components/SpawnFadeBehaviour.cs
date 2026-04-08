using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

public sealed class SpawnFadeBehaviour : IComponent
{
    private int _timeUntilStart;
    private readonly int _spawnDelay;

    public SpawnFadeBehaviour(int spawnDelay)
    {
        _spawnDelay = spawnDelay;
        _timeUntilStart = spawnDelay;
    }

    public void OnAdded(Entity owner) { }

    public void Update(Entity owner)
    {
        if (_timeUntilStart > 0)
        {
            _timeUntilStart--;
            owner.Tint = Color.White * (1 - _timeUntilStart / (float)_spawnDelay);
        }
        
        if (owner is Enemy enemy)
        {
            enemy.IsActive = _timeUntilStart <= 0;
        }
    }
}
