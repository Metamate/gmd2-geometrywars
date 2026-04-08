namespace GeometryWars.Entities;

// Archetype for black hole hazards.
public class BlackHole : Entity
{
    public void Kill() => IsExpired = true;
}
