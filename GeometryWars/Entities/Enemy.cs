using System;

namespace GeometryWars.Entities;

// Archetype for enemy units.
public sealed class Enemy : Entity
{
    public int PointValue { get; }

    public event Action<Enemy> Killed;

    public Enemy(int pointValue) => PointValue = pointValue;

    public void Kill()
    {
        if (IsExpired) return;

        Killed?.Invoke(this);
        IsExpired = true;
    }
}
