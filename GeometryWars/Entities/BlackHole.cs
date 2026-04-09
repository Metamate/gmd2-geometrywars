namespace GeometryWars.Entities;

// Archetype for black hole hazards.
public sealed class BlackHole : Entity
{
    public void Kill() => IsExpired = true;
}
