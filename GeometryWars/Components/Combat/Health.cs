using System;
using GMDCore.ECS.Components;

namespace GeometryWars.Components.Combat;

// Stores hit points that other components can damage or inspect.
public sealed class Health : Component
{
    public int Current { get; private set; }
    public bool IsDepleted => Current <= 0;

    public event Action Damaged;
    public event Action Depleted;

    public Health(int startingHealth)
    {
        Current = Math.Max(0, startingHealth);
    }

    public void ApplyDamage(int damage)
    {
        if (damage <= 0 || IsDepleted)
            return;

        Current = Math.Max(0, Current - damage);
        Damaged?.Invoke();

        if (IsDepleted)
            Depleted?.Invoke();
    }
}

