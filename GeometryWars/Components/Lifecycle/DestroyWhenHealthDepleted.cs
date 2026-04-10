using GeometryWars.Components.Combat;
using GeometryWars.Components.Core;
using GeometryWars.Entities;

namespace GeometryWars.Components.Lifecycle;

// Destroys the owner once its health has been reduced to zero.
public sealed class DestroyWhenHealthDepleted : Component
{
    private Destroyable _destroyable;
    private Health _health;

    public override void OnStart(Entity owner)
    {
        _destroyable = owner.RequireComponent<Destroyable>();
        _health = owner.RequireComponent<Health>();
        _health.Depleted += HandleHealthDepleted;
    }

    public override void OnRemoved(Entity owner)
    {
        if (_health != null)
            _health.Depleted -= HandleHealthDepleted;
    }

    private void HandleHealthDepleted()
    {
        _destroyable?.Destroy();
    }
}
