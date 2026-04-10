using GMDCore.ECS.Components;
using GMDCore.ECS;

namespace GeometryWars.Components.Lifecycle;

// Automatically marks an entity as expired after a fixed number of frames.
// At 60Hz, 60 frames = 1 second.
public sealed class LifetimeExpiry : Component
{
    private int _framesRemaining;

    public LifetimeExpiry(int frames)
    {
        _framesRemaining = frames;
    }

    public override void PostUpdate(Entity owner)
    {
        if (--_framesRemaining <= 0)
            owner.IsExpired = true;
    }
}

