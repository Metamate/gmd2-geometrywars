using System;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

public sealed class SprayBehaviour : IComponent
{
    private float _sprayAngle = 0;
    private readonly float _gridRange;

    public SprayBehaviour(float gridRange)
    {
        _gridRange = gridRange;
    }

    public void OnAdded(Entity owner) { }

    public void Update(Entity owner)
    {
        if ((FrameContext.Time.TotalGameTime.Milliseconds / 250) % 2 == 0)
        {
            Vector2 sprayVel = MathUtil.FromPolar(_sprayAngle, Random.Shared.NextFloat(12, 15));
            Color color = ColorUtil.HSVToColor(5, 0.5f, 0.8f);
            Vector2 pos = owner.Position + 2f * new Vector2(sprayVel.Y, -sprayVel.X) + Random.Shared.NextVector2(4, 8);
            GameServices.Particles.CreateParticle(Art.LineParticle, pos, color, 190, 1.5f,
                new ParticleState { Velocity = sprayVel, LengthMultiplier = 1, Type = ParticleType.Enemy });
        }

        _sprayAngle -= MathHelper.TwoPi / 50f;
        GameServices.Grid.ApplyImplosiveForce((float)Math.Sin(_sprayAngle / 2) * 10 + 20, owner.Position, _gridRange);
    }
}
