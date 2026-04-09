using GeometryWars.Components.Core;
using GeometryWars.Components.Visuals;
using GeometryWars.Entities;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.Lifecycle;

// Handles the initial fade-in and sibling activation after the spawn window.
public sealed class FadeInOnSpawn : Component
{
    private int _timeUntilStart;
    private readonly int _spawnDelay;
    private Sprite _sprite;

    public FadeInOnSpawn(int spawnDelay)
    {
        _spawnDelay = spawnDelay;
        _timeUntilStart = spawnDelay;
    }

    // OnStart runs after EntityWorld calls entity.Start(), which happens once all
    // components have been added. Every sibling — including those from factory methods
    // — is present, so we can safely disable them all here.
    public override void OnStart(Entity owner)
    {
        _sprite = owner.GetComponent<Sprite>();

        foreach (var comp in owner.Components)
        {
            if (comp == this || comp is Sprite || comp is Transform)
                continue;

            comp.IsActive = false;
        }
    }

    public override void PreUpdate(Entity owner)
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
