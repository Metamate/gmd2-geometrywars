using GeometryWars.Components.Combat;
using GeometryWars.Components.Core;
using GeometryWars.Entities;

namespace GeometryWars.Components.Lifecycle;

// Destroys the owner once its health has been reduced to zero.
public sealed class DestroyWhenHealthDepletedComponent : Component
{
    private DestroyableComponent _destroyable;
    private HealthComponent _health;

    public override void OnStart(Entity owner)
    {
        if (_health != null)
            _health.Depleted -= HandleHealthDepleted;

        _destroyable = owner.GetComponent<DestroyableComponent>();
        _health = owner.GetComponent<HealthComponent>();
        if (_health != null)
            _health.Depleted += HandleHealthDepleted;
    }

    private void HandleHealthDepleted()
    {
        _destroyable?.Destroy();
    }
}
