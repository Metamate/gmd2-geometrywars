using GeometryWars.Components.Core;
using GeometryWars.Entities;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.Physics;

// Wraps an entity to the opposite edge when it leaves the screen.
// Alternative to ScreenClampBehaviour: clamp stops at the edge, wrap teleports.
// Not currently assigned to any entity — kept as a teaching alternative.
public sealed class ScreenWrap : Component
{
    private readonly GameRuntime _runtime;
    private TransformComponent _transform;

    public ScreenWrap(GameRuntime runtime) => _runtime = runtime;

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
    }

    public override void PostUpdate(Entity owner)
    {
        var pos = _transform.Position;
        var size = _runtime.Frame.ScreenSize;

        if (pos.X < 0) pos.X += size.X;
        else if (pos.X > size.X) pos.X -= size.X;

        if (pos.Y < 0) pos.Y += size.Y;
        else if (pos.Y > size.Y) pos.Y -= size.Y;

        _transform.Position = pos;
    }
}
