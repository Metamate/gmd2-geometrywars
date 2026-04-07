namespace GeometryWars.Components;

// Expires the entity automatically after a fixed number of frames.
// Replaces manual countdown fields on entities that have a fixed lifespan,
// keeping the entity class free of timer boilerplate.
//
// Usage: AddComponent(new LifetimeExpiry(frames: 180)); // 3 seconds at 60 fps
//
// Good for: power-ups that disappear after a few seconds, temporary explosion
// debris, timed obstacles, or any entity that should self-destruct on a timer.
// Combine with VelocityMover for a self-propelled temporary entity.
public sealed class LifetimeExpiry : IComponent
{
    private int _framesLeft;

    public LifetimeExpiry(int frames) => _framesLeft = frames;

    public void Update(Entity owner)
    {
        if (--_framesLeft <= 0)
            owner.IsExpired = true;
    }
}
