namespace GeometryWars.Components.Core;

// Explicit per-entity update phases. Keeping these few and coarse makes the
// runtime order easy to teach without introducing a full scheduler.
public enum ComponentUpdatePhase
{
    PreUpdate = 0,
    Logic = 1,
    Physics = 2,
    PostPhysics = 3,
}
