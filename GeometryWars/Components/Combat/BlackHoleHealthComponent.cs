using GeometryWars.Components.Core;
using GeometryWars.Components.Visuals;
using GeometryWars.Entities;

namespace GeometryWars.Components.Combat;

// Owns black hole hitpoints and delegates hit feedback to a dedicated effects component.
public sealed class BlackHoleHealthComponent : Component
{
    private int _hitpoints;
    private TransformComponent _transform;
    private BlackHoleHitEffectsComponent _hitEffects;

    public BlackHoleHealthComponent(int hitpoints)
    {
        _hitpoints = hitpoints;
    }

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
        _hitEffects = owner.GetComponent<BlackHoleHitEffectsComponent>();
    }

    public override void OnCollision(Entity owner, Entity other)
    {
        if (owner.IsExpired || other is not Bullet)
            return;

        _hitEffects?.PlayHit(_transform.Position);

        if (--_hitpoints <= 0)
            owner.IsExpired = true;
    }
}
