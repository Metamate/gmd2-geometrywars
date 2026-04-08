using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

public sealed class SpawnFadeBehaviour : Component
{
    private int _timeUntilStart;
    private readonly int _spawnDelay;
    private SpriteComponent _sprite;

    public SpawnFadeBehaviour(int spawnDelay)
    {
        _spawnDelay = spawnDelay;
        _timeUntilStart = spawnDelay;
    }

    public override void OnAdded(Entity owner)
    {
        _sprite = owner.GetComponent<SpriteComponent>();
        
        owner.SetAllComponentsActive(false);
        IsActive = true; 
        
        if (_sprite != null) _sprite.IsActive = true;
    }

    public override void Update(Entity owner)
    {
        if (_timeUntilStart > 0)
        {
            _timeUntilStart--;
            
            if (_sprite != null)
            {
                _sprite.Tint = Color.White * (1 - _timeUntilStart / (float)_spawnDelay);
            }

            if (_timeUntilStart <= 0)
            {
                owner.SetAllComponentsActive(true);
                
                IsActive = false; 
            }
        }
    }
}
