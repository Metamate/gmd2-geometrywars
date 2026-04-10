using System;
using GMDCore.ECS.Components;
using GMDCore.ECS;
using GeometryWars.Systems;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.Physics;

// Applies an oscillating implosive force to the background grid around the owner.
public sealed class ApplyOscillatingImplosiveGridForce : Component
{
    private float _pulseAngle;
    private readonly float _gridRange;
    private readonly IGridField _grid;
    private Transform _transform;

    public ApplyOscillatingImplosiveGridForce(float gridRange, IGridField grid)
    {
        _gridRange = gridRange;
        _grid = grid;
    }

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
    }

    public override void Update(Entity owner)
    {
        _pulseAngle -= MathHelper.TwoPi / 50f;
        _grid.ApplyImplosiveForce((float)Math.Sin(_pulseAngle / 2) * 10 + 20, _transform.Position, _gridRange);
    }
}

