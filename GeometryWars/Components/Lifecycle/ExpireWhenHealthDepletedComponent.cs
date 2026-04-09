using GeometryWars.Components.Combat;
using GeometryWars.Components.Core;
using GeometryWars.Entities;

namespace GeometryWars.Components.Lifecycle;

// Expires the owner once its health has been reduced to zero.
public sealed class ExpireWhenHealthDepletedComponent : Component
{
    private HealthComponent _health;
    private Entity _owner;

    public override void OnStart(Entity owner)
    {
        if (_health != null)
            _health.Depleted -= HandleDepleted;

        _owner = owner;
        _health = owner.GetComponent<HealthComponent>();
        if (_health != null)
            _health.Depleted += HandleDepleted;
    }

    private void HandleDepleted()
    {
        _owner.IsExpired = true;
    }
}
