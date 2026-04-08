using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

/// <summary>
/// Component that handles the initial fade-in and state activation.
/// Now interacts with SpriteComponent to apply visual fading.
/// </summary>
public sealed class SpawnFadeBehaviour : IComponent
{
    private int _timeUntilStart;
    private readonly int _spawnDelay;
    private SpriteComponent _sprite;

    public SpawnFadeBehaviour(int spawnDelay)
    {
        _spawnDelay = spawnDelay;
        _timeUntilStart = spawnDelay;
    }

    public void OnAdded(Entity owner)
    {
        _sprite = owner.GetComponent<SpriteComponent>();
    }

    public void Update(Entity owner)
    {
        _sprite ??= owner.GetComponent<SpriteComponent>();

        if (_timeUntilStart > 0)
        {
            _timeUntilStart--;
            
            if (_sprite != null)
            {
                _sprite.Tint = Color.White * (1 - _timeUntilStart / (float)_spawnDelay);
            }
        }
        
        if (owner is Enemy enemy)
        {
            enemy.IsActive = _timeUntilStart <= 0;
        }
    }
}
