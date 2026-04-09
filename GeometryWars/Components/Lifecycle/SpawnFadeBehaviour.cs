using GeometryWars.Components.Core;
using GeometryWars.Components.Visuals;
using GeometryWars.Entities;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.Lifecycle;

// Handles the initial fade-in and sibling activation after the spawn window.
public sealed class SpawnFadeBehaviour : Component
{
    public override ComponentUpdatePhase Phase => ComponentUpdatePhase.PreUpdate;

    private int _timeUntilStart;
    private readonly int _spawnDelay;
    private SpriteComponent _sprite;

    public SpawnFadeBehaviour(int spawnDelay)
    {
        _spawnDelay = spawnDelay;
        _timeUntilStart = spawnDelay;
    }

    // OnStart runs after EntityWorld calls entity.Start(), which happens once all
    // components have been added. Every sibling — including those from factory methods
    // — is present, so we can safely disable them all here.
    public override void OnStart(Entity owner)
    {
        _sprite = owner.GetComponent<SpriteComponent>();

        foreach (var comp in owner.Components)
        {
            if (comp == this || comp is SpriteComponent || comp is TransformComponent)
                continue;

            comp.IsActive = false;
        }
    }

    public override void Update(Entity owner)
    {
        if (_timeUntilStart <= 0) return;

        _timeUntilStart--;

        if (_sprite != null)
            _sprite.Tint = Color.White * (1 - _timeUntilStart / (float)_spawnDelay);

        if (_timeUntilStart <= 0)
        {
            foreach (var comp in owner.Components)
                comp.IsActive = true;

            IsActive = false;
        }
    }
}
