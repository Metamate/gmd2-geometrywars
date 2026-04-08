using GeometryWars.Components.Core;
using GeometryWars.Components.Visuals;
using GeometryWars.Entities;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.Lifecycle;

// Handles the initial fade-in and component activation.
//
// Design note: siblings are disabled on the FIRST Update, not in OnAdded.
// This ensures that components added after SpawnFadeBehaviour (e.g. AI behaviours
// added by factory methods) are also correctly disabled during the spawn window.
public sealed class SpawnFadeBehaviour : Component
{
    private int _timeUntilStart;
    private readonly int _spawnDelay;
    private SpriteComponent _sprite;
    private bool _initialized;

    public SpawnFadeBehaviour(int spawnDelay)
    {
        _spawnDelay = spawnDelay;
        _timeUntilStart = spawnDelay;
    }

    public override void OnStart(Entity owner)
    {
        _sprite = owner.GetComponent<SpriteComponent>();
    }

    public override void Update(Entity owner)
    {
        if (!_initialized)
        {
            _initialized = true;

            // Disable all sibling components so the entity is inert during spawn.
            // We do this here rather than in OnAdded so that components added after
            // us (e.g. SeekBehaviour from a factory method) are also caught.
            foreach (var comp in owner.Components)
            {
                if (comp == this || comp is SpriteComponent || comp is TransformComponent)
                    continue;

                comp.IsActive = false;
            }
        }

        if (_timeUntilStart > 0)
        {
            _timeUntilStart--;

            if (_sprite != null)
                _sprite.Tint = Color.White * (1 - _timeUntilStart / (float)_spawnDelay);

            if (_timeUntilStart <= 0)
            {
                // Re-enable all components once birth is complete.
                foreach (var comp in owner.Components)
                    comp.IsActive = true;

                IsActive = false;
            }
        }
    }
}
