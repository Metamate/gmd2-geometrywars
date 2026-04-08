using System;
using GeometryWars.Components.Lifecycle;

namespace GeometryWars.Entities;

// Archetype for enemy units.
public class Enemy : Entity
{
    public int PointValue { get; }

    public Enemy(int pointValue) => PointValue = pointValue;

    public void Kill()
    {
        if (IsExpired) return;

        GetComponent<EnemyDeathEffectsComponent>()?.Play(this);
        IsExpired = true;
    }
}
