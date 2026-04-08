using GeometryWars.Components.Core;
using GeometryWars.Components.Visuals;
using GeometryWars.Entities;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.Lifecycle;

// Handles the initial fade-in and component activation.
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
        
        // Disable sibling components while spawning.
        foreach (var comp in owner.Components)
        {
            if (comp == this || comp is SpriteComponent || comp is TransformComponent) 
                continue;
            
            comp.IsActive = false;
        }
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
                // Re-enable all components once birth is complete.
                foreach (var comp in owner.Components)
                {
                    comp.IsActive = true;
                }
                
                this.IsActive = false; 
            }
        }
    }
}
