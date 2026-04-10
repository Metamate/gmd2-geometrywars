using System;
using System.Collections.Generic;
using GeometryWars.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Systems;

// Dense spring grid stored in flat arrays so the hot update path has better
// data locality than the gameplay ECS layer.
public sealed class Grid : IGridField
{
    private const float PointDamping = 0.98f;
    private const float RestLengthFactor = 0.95f;

    private readonly SpringData[] _springs;
    private readonly PointMassData[] _points;
    private readonly Vector2[] _projectedPoints;
    private readonly Vector2 _screenCenter;
    private readonly int _cols;
    private readonly int _rows;

    public Grid(Rectangle size, Vector2 spacing)
    {
        _screenCenter = new Vector2(size.Center.X, size.Center.Y);
        _cols = (int)(size.Width / spacing.X) + 1;
        _rows = (int)(size.Height / spacing.Y) + 1;

        _points = new PointMassData[_cols * _rows];
        _projectedPoints = new Vector2[_points.Length];

        float gridWidth = (_cols - 1) * spacing.X;
        float gridHeight = (_rows - 1) * spacing.Y;
        float originX = size.Left + (size.Width - gridWidth) * 0.5f;
        float originY = size.Top + (size.Height - gridHeight) * 0.5f;

        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _cols; x++)
            {
                int index = Index(x, y);
                _points[index] = new PointMassData(new Vector3(originX + x * spacing.X, originY + y * spacing.Y, 0), inverseMass: 1f);
            }
        }

        var springList = new List<SpringData>();
        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _cols; x++)
            {
                int index = Index(x, y);
                var pointPosition = _points[index].Position;

                if (x == 0 || y == 0 || x == _cols - 1 || y == _rows - 1)
                    springList.Add(SpringData.CreateFixed(index, pointPosition, stiffness: 0.1f, damping: 0.1f));
                else if (x % 3 == 0 && y % 3 == 0)
                    springList.Add(SpringData.CreateFixed(index, pointPosition, stiffness: 0.002f, damping: 0.02f));

                const float stiffness = 0.28f;
                const float damping = 0.06f;
                if (x > 0) springList.Add(CreateSpring(Index(x - 1, y), index, stiffness, damping));
                if (y > 0) springList.Add(CreateSpring(Index(x, y - 1), index, stiffness, damping));
            }
        }

        _springs = [.. springList];
    }

    public void Update()
    {
        for (int i = 0; i < _springs.Length; i++)
            UpdateSpring(in _springs[i]);

        for (int i = 0; i < _points.Length; i++)
            UpdatePoint(i);
    }

    public void ApplyDirectedForce(Vector2 force, Vector2 position, float radius)
    {
        var force3 = new Vector3(force, 0);
        var pos3 = new Vector3(position, 0);
        float radiusSq = radius * radius;

        for (int i = 0; i < _points.Length; i++)
        {
            ref var point = ref _points[i];
            float distanceSq = Vector3.DistanceSquared(pos3, point.Position);
            if (distanceSq >= radiusSq)
                continue;

            ApplyForce(i, 10 * force3 / (10 + MathF.Sqrt(distanceSq)));
        }
    }

    public void ApplyImplosiveForce(float force, Vector2 position, float radius)
    {
        var pos3 = new Vector3(position, 0);
        float radiusSq = radius * radius;

        for (int i = 0; i < _points.Length; i++)
        {
            ref var point = ref _points[i];
            float distanceSq = Vector3.DistanceSquared(pos3, point.Position);
            if (distanceSq >= radiusSq)
                continue;

            ApplyForce(i, 10 * force * (pos3 - point.Position) / (100 + distanceSq));
            IncreaseDamping(i, 0.6f);
        }
    }

    public void ApplyExplosiveForce(float force, Vector2 position, float radius)
    {
        var pos3 = new Vector3(position, 0);
        float radiusSq = radius * radius;

        for (int i = 0; i < _points.Length; i++)
        {
            ref var point = ref _points[i];
            float distanceSq = Vector3.DistanceSquared(pos3, point.Position);
            if (distanceSq >= radiusSq)
                continue;

            ApplyForce(i, 100 * force * (point.Position - pos3) / (10000 + distanceSq));
            IncreaseDamping(i, 0.6f);
        }
    }

    public void ApplyDepthPulse(Vector2 position, float force, float radius)
    {
        var force3 = new Vector3(0, 0, force);
        var pos3 = new Vector3(position, 0);
        float radiusSq = radius * radius;

        for (int i = 0; i < _points.Length; i++)
        {
            ref var point = ref _points[i];
            float distanceSq = Vector3.DistanceSquared(pos3, point.Position);
            if (distanceSq >= radiusSq)
                continue;

            ApplyForce(i, 10 * force3 / (10 + MathF.Sqrt(distanceSq)));
        }
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
    {
        UpdateProjectedPoints();

        Color color = new(30, 30, 139, 85);
        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _cols; x++)
            {
                int index = Index(x, y);
                Vector2 point = _projectedPoints[index];

                if (x > 0)
                {
                    Vector2 left = _projectedPoints[Index(x - 1, y)];
                    float thickness = y % 3 == 1 ? 3f : 1f;

                    if (x == 1)
                    {
                        spriteBatch.DrawLine(pixel, left, point, color, thickness);
                    }
                    else
                    {
                        int nextIndex = Index(Math.Min(x + 1, _cols - 1), y);
                        Vector2 next = _projectedPoints[nextIndex];
                        Vector2 previous = _projectedPoints[Index(x - 2, y)];
                        Vector2 midpoint = Vector2.CatmullRom(previous, left, point, next, 0.5f);

                        if (Vector2.DistanceSquared(midpoint, (left + point) * 0.5f) > 1f)
                        {
                            spriteBatch.DrawLine(pixel, left, midpoint, color, thickness);
                            spriteBatch.DrawLine(pixel, midpoint, point, color, thickness);
                        }
                        else
                        {
                            spriteBatch.DrawLine(pixel, left, point, color, thickness);
                        }
                    }
                }

                if (y > 0)
                {
                    Vector2 up = _projectedPoints[Index(x, y - 1)];
                    float thickness = x % 3 == 1 ? 3f : 1f;
                    spriteBatch.DrawLine(pixel, up, point, color, thickness);
                }

                if (x > 0 && y > 0)
                {
                    Vector2 up = _projectedPoints[Index(x, y - 1)];
                    Vector2 left = _projectedPoints[Index(x - 1, y)];
                    Vector2 upLeft = _projectedPoints[Index(x - 1, y - 1)];
                    spriteBatch.DrawLine(pixel, 0.5f * (upLeft + up), 0.5f * (left + point), color, 1f);
                    spriteBatch.DrawLine(pixel, 0.5f * (upLeft + left), 0.5f * (up + point), color, 1f);
                }
            }
        }
    }

    private int Index(int x, int y) => x + y * _cols;

    private SpringData CreateSpring(int end1, int end2, float stiffness, float damping)
    {
        float targetLength = Vector3.Distance(_points[end1].Position, _points[end2].Position) * RestLengthFactor;
        return SpringData.CreateDynamic(end1, end2, targetLength, stiffness, damping);
    }

    private void UpdateProjectedPoints()
    {
        for (int i = 0; i < _points.Length; i++)
            _projectedPoints[i] = ToVec2(_points[i].Position);
    }

    private Vector2 ToVec2(Vector3 position)
    {
        float factor = (position.Z + 2000) / 2000;
        return (new Vector2(position.X, position.Y) - _screenCenter) * factor + _screenCenter;
    }

    private void UpdateSpring(in SpringData spring)
    {
        Vector3 end1Position = spring.ToFixed ? spring.FixedPosition : _points[spring.End1].Position;
        Vector3 delta = end1Position - _points[spring.End2].Position;
        float length = delta.Length();

        if (length <= spring.TargetLength || length < 0.0001f)
            return;

        delta = delta / length * (length - spring.TargetLength);
        Vector3 velocityDelta = _points[spring.End2].Velocity - (spring.ToFixed ? Vector3.Zero : _points[spring.End1].Velocity);
        Vector3 force = spring.Stiffness * delta - velocityDelta * spring.Damping;

        if (!spring.ToFixed)
            ApplyForce(spring.End1, -force);

        ApplyForce(spring.End2, force);
    }

    private void UpdatePoint(int index)
    {
        ref var point = ref _points[index];
        point.Velocity += point.Acceleration;
        point.Position += point.Velocity;
        point.Acceleration = Vector3.Zero;

        if (point.Velocity.LengthSquared() < 0.000001f)
            point.Velocity = Vector3.Zero;

        point.Velocity *= point.Damping;
        point.Damping = PointDamping;
    }

    private void ApplyForce(int index, Vector3 force)
    {
        ref var point = ref _points[index];
        point.Acceleration += force * point.InverseMass;
    }

    private void IncreaseDamping(int index, float factor)
    {
        ref var point = ref _points[index];
        point.Damping *= factor;
    }

    private struct PointMassData
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public Vector3 Acceleration;
        public float InverseMass;
        public float Damping;

        public PointMassData(Vector3 position, float inverseMass)
        {
            Position = position;
            Velocity = Vector3.Zero;
            Acceleration = Vector3.Zero;
            InverseMass = inverseMass;
            Damping = PointDamping;
        }
    }

    private readonly struct SpringData
    {
        public int End1 { get; }
        public int End2 { get; }
        public float TargetLength { get; }
        public float Stiffness { get; }
        public float Damping { get; }
        public bool ToFixed { get; }
        public Vector3 FixedPosition { get; }

        private SpringData(int end1, int end2, float targetLength, float stiffness, float damping, bool toFixed, Vector3 fixedPosition)
        {
            End1 = end1;
            End2 = end2;
            TargetLength = targetLength;
            Stiffness = stiffness;
            Damping = damping;
            ToFixed = toFixed;
            FixedPosition = fixedPosition;
        }

        public static SpringData CreateFixed(int index, Vector3 fixedPosition, float stiffness, float damping)
            => new(index, index, 0f, stiffness, damping, true, fixedPosition);

        public static SpringData CreateDynamic(int end1, int end2, float targetLength, float stiffness, float damping)
            => new(end1, end2, targetLength, stiffness, damping, false, Vector3.Zero);
    }
}
