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
        if (_health != null)
            _health.Depleted -= HandleHealthDepleted;

        _destroyable = owner.GetComponent<Destroyable>();
        _health = owner.GetComponent<Health>();
        if (_health != null)
            _health.Depleted += HandleHealthDepleted;
    }

    private void HandleHealthDepleted()
    {
        _destroyable?.Destroy();
    }
}
