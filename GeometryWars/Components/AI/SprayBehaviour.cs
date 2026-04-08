using System;
using GeometryWars.Components.Core;
using GeometryWars.Entities;
using GeometryWars.Systems;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.AI;

// Spawns orbital particles and applies grid distortion.
public sealed class SprayBehaviour : Component
{
    private float _sprayAngle;
    private readonly float _gridRange;
    private readonly IParticleSystem<ParticleState> _particles;
    private readonly IGridField _grid;
    private TransformComponent _transform;

    public SprayBehaviour(float gridRange, IParticleSystem<ParticleState> particles, IGridField grid)
    {
        _gridRange = gridRange;
        _particles = particles;
        _grid = grid;
    }

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
    }

    public override void Update(Entity owner)
    {
        if (((int)FrameContext.Time.TotalGameTime.TotalMilliseconds / 250) % 2 == 0)
        {
            Vector2 sprayVel = MathUtil.FromPolar(_sprayAngle, Random.Shared.NextFloat(12, 15));
            Color color = ColorUtil.HSVToColor(5, 0.5f, 0.8f);
            Vector2 pos = _transform.Position + 2f * new Vector2(sprayVel.Y, -sprayVel.X) + Random.Shared.NextVector2(4, 8);
            _particles.CreateParticle(Art.LineParticle, pos, color, 190, 1.5f,
                new ParticleState { Velocity = sprayVel, LengthMultiplier = 1, Type = ParticleType.Enemy });
        }

        _sprayAngle -= MathHelper.TwoPi / 50f;
        _grid.ApplyImplosiveForce((float)Math.Sin(_sprayAngle / 2) * 10 + 20, _transform.Position, _gridRange);
    }
}
