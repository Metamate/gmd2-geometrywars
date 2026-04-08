using GeometryWars.Components.Core;
using GeometryWars.Components.Visuals;
using GeometryWars.Entities;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.Lifecycle;

/// <summary>
/// Component that handles the initial fade-in.
/// It keeps other components inactive until the fade is complete.
/// </summary>
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
        
        // GATING: Disable all sibling components while spawning.
        owner.SetAllComponentsActive(false);
        this.IsActive = true; 
        
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
                // Birth complete: activate all capabilities
                owner.SetAllComponentsActive(true);
                
                // We are no longer needed
                this.IsActive = false; 
            }
        }
    }
}
