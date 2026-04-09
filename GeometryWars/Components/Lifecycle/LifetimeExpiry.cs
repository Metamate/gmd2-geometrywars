using GeometryWars.Components.Core;
using GeometryWars.Entities;

namespace GeometryWars.Components.Lifecycle;

// Automatically marks an entity as expired after a fixed number of frames.
// At 60Hz, 60 frames = 1 second.
public sealed class LifetimeExpiry : Component
{
    public override ComponentUpdatePhase Phase => ComponentUpdatePhase.PostPhysics;

    private int _framesRemaining;

    public LifetimeExpiry(int frames)
    {
        _framesRemaining = frames;
    }

    public override void Update(Entity owner)
    {
        if (--_framesRemaining <= 0)
            owner.IsExpired = true;
    }
}
