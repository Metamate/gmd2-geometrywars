using GeometryWars.Components.Combat;
using GeometryWars.Components.Core;
using GeometryWars.Entities;

namespace GeometryWars.Components.Lifecycle;

// Expires the owner once its health has been reduced to zero.
public sealed class ExpireWhenHealthDepletedComponent : Component
{
    private HealthComponent _health;

    public override void OnStart(Entity owner)
    {
        _health = owner.GetComponent<HealthComponent>();
    }

    public override void PostUpdate(Entity owner)
    {
        if (_health?.IsDepleted == true)
            owner.IsExpired = true;
    }
}
