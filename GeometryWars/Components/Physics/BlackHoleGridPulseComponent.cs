using System;
using GeometryWars.Components.Core;
using GeometryWars.Entities;
using GeometryWars.Systems;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.Physics;

// Applies the black hole's oscillating pull to the background grid.
public sealed class BlackHoleGridPulseComponent : Component
{
    private float _pulseAngle;
    private readonly float _gridRange;
    private readonly IGridField _grid;
    private TransformComponent _transform;

    public BlackHoleGridPulseComponent(float gridRange, IGridField grid)
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
