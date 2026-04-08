namespace GeometryWars.Components;

public sealed class LifetimeExpiry : Component
{
    private int _framesRemaining;

    public LifetimeExpiry(int frames)
    {
        _framesRemaining = frames;
    }

    public override void Update(Entity owner)
    {
        if (--_framesRemaining <= 0)
        {
            owner.IsExpired = true;
        }
    }
}
