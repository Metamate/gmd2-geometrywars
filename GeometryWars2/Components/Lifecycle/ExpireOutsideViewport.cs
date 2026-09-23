using GMDCore.ECS.Components;
using GMDCore.ECS;
using GeometryWars2.Services;

namespace GeometryWars2.Components.Lifecycle;

// Expires an entity when it leaves the viewport.
public sealed class ExpireOutsideViewport : Component
{
    private readonly FrameInfo _frame;
    private Transform _transform;

    public ExpireOutsideViewport(FrameInfo frame)
    {
        _frame = frame;
    }

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
    }

    public override void PostUpdate(Entity owner)
    {
        if (!owner.IsExpired && !_frame.Viewport.Bounds.Contains(_transform.Position.ToPoint()))
            owner.IsExpired = true;
    }
}
