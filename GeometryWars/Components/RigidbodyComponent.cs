using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

public sealed class RigidbodyComponent : Component
{
    public Vector2 Velocity { get; set; }
    
    private TransformComponent _transform;
    private readonly float _damping;

    public RigidbodyComponent(float damping = 1f)
    {
        _damping = damping;
    }

    public override void OnAdded(Entity owner)
    {
        _transform = owner.Transform;
    }

    public override void Update(Entity owner)
    {
        _transform.Position += Velocity;
        Velocity *= _damping;
    }
}
