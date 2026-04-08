using GeometryWars.Components.Core;
using GeometryWars.Entities;

namespace GeometryWars.Components.Lifecycle;

// Automatically marks an entity as expired after a duration.
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
