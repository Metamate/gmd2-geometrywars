namespace GeometryWars.Components;

/// <summary>
/// Component that automatically marks an entity as expired after a set duration.
/// </summary>
public sealed class LifetimeExpiry : IComponent
{
    private int _framesRemaining;

    public LifetimeExpiry(int frames)
    {
        _framesRemaining = frames;
    }

    public void OnAdded(Entity owner) { }

    public void Update(Entity owner)
    {
        if (--_framesRemaining <= 0)
        {
            owner.IsExpired = true;
        }
    }
}
