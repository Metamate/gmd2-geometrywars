using System;
using GMDCore.ECS.Components;
using GMDCore.ECS;

namespace GeometryWars.Components.Lifecycle;

// Provides a reusable destruction signal for entities that need "on destroyed" behavior.
public sealed class Destroyable : Component
{
    private Entity _owner;

    public event Action<Entity> Destroyed;

    public override void OnStart(Entity owner)
    {
        _owner = owner;
    }

    public void Destroy()
    {
        if (_owner == null || _owner.IsExpired)
            return;

        Destroyed?.Invoke(_owner);
        _owner.IsExpired = true;
    }
}

