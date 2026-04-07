using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

/// <summary>
/// ADVANCED: A high-performance version of the background grid.
/// 
/// Educational Value (Data Oriented Design):
/// 1. Data Locality: PointMass is a struct stored in a flat 1D array. This ensures the
///    CPU can stream the data into the cache without chasing pointers.
/// 2. Memory Compactness: No separate heap objects for thousands of points.
/// 3. Direct Access: Uses the 'ref' keyword to modify array elements in-place,
///    avoiding the overhead of copying structs by value.
/// 4. Pointer-less Relationships: Springs store indices (int) instead of references.
/// </summary>
public class GridOptimized
{
    private readonly Spring[] _springs;
    private readonly PointMass[] _points;
    private readonly PointMass[] _fixedPoints;
    private readonly int _cols;
    private readonly int _rows;
    private readonly Vector2 _screenSize;

    public GridOptimized(Rectangle size, Vector2 spacing)
    {
        _screenSize = new Vector2(size.Width, size.Height);
        _cols = (int)(size.Width / spacing.X) + 1;
        _rows = (int)(size.Height / spacing.Y) + 1;

        int totalPoints = _cols * _rows;
        _points = new PointMass[totalPoints];
        _fixedPoints = new PointMass[totalPoints];

        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _cols; x++)
            {
                int index = y * _cols + x;
                Vector3 pos = new(size.Left + x * spacing.X, size.Top + y * spacing.Y, 0);
                _points[index] = new PointMass(pos, 1f);
                _fixedPoints[index] = new PointMass(pos, 0f);
            }
        }

        var springList = new List<Spring>();
        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _cols; x++)
            {
                int index = y * _cols + x;

                if (x == 0 || y == 0 || x == _cols - 1 || y == _rows - 1)
                    springList.Add(CreateSpring(index, index, 0.1f, 0.1f, true));
                else if (x % 3 == 0 && y % 3 == 0)
                    springList.Add(CreateSpring(index, index, 0.002f, 0.02f, true));

                const float stiffness = 0.28f;
                const float damping = 0.06f;
                if (x > 0) springList.Add(CreateSpring(index - 1, index, stiffness, damping));
                if (y > 0) springList.Add(CreateSpring(index - _cols, index, stiffness, damping));
            }
        }
        _springs = [.. springList];
    }

    private Spring CreateSpring(int i1, int i2, float stiffness, float damping, bool toFixed = false)
    {
        Vector3 p1 = toFixed ? _fixedPoints[i1].Position : _points[i1].Position;
        Vector3 p2 = _points[i2].Position;
        float targetLength = Vector3.Distance(p1, p2) * 0.95f;
        return new Spring(i1, i2, targetLength, stiffness, damping, toFixed);
    }

    public void Update()
    {
        for (int i = 0; i < _springs.Length; i++)
            _springs[i].Update(_points, _fixedPoints);

        for (int i = 0; i < _points.Length; i++)
            _points[i].Update();
    }

    public void ApplyDirectedForce(Vector2 force, Vector2 position, float radius)
        => ApplyDirectedForce(new Vector3(force, 0), new Vector3(position, 0), radius);

    public void ApplyDirectedForce(Vector3 force, Vector3 position, float radius)
    {
        float rSq = radius * radius;
        for (int i = 0; i < _points.Length; i++)
        {
            float distSq = Vector3.DistanceSquared(position, _points[i].Position);
            if (distSq < rSq)
                _points[i].ApplyForce(10 * force / (10 + MathF.Sqrt(distSq)));
        }
    }

    public void ApplyImplosiveForce(float force, Vector2 position, float radius)
        => ApplyImplosiveForce(force, new Vector3(position, 0), radius);

    public void ApplyImplosiveForce(float force, Vector3 position, float radius)
    {
        float rSq = radius * radius;
        for (int i = 0; i < _points.Length; i++)
        {
            float distSq = Vector3.DistanceSquared(position, _points[i].Position);
            if (distSq < rSq)
            {
                _points[i].ApplyForce(10 * force * (position - _points[i].Position) / (100 + distSq));
                _points[i].IncreaseDamping(0.6f);
            }
        }
    }

    public void ApplyExplosiveForce(float force, Vector2 position, float radius)
        => ApplyExplosiveForce(force, new Vector3(position, 0), radius);

    public void ApplyExplosiveForce(float force, Vector3 position, float radius)
    {
        float rSq = radius * radius;
        for (int i = 0; i < _points.Length; i++)
        {
            float distSq = Vector3.DistanceSquared(position, _points[i].Position);
            if (distSq < rSq)
            {
                _points[i].ApplyForce(100 * force * (_points[i].Position - position) / (10000 + distSq));
                _points[i].IncreaseDamping(0.6f);
            }
        }
    }

    private Vector2 ToVec2(Vector3 v)
    {
        float factor = (v.Z + 2000) / 2000;
        return (new Vector2(v.X, v.Y) - _screenSize / 2f) * factor + _screenSize / 2;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Color color = new(30, 30, 139, 85);
        for (int y = 1; y < _rows; y++)
        {
            for (int x = 1; x < _cols; x++)
            {
                int index = y * _cols + x;
                Vector2 p = ToVec2(_points[index].Position);

                if (x > 1)
                {
                    Vector2 left = ToVec2(_points[index - 1].Position);
                    float thickness = y % 3 == 1 ? 3f : 1f;

                    // Catmull-Rom: smooth the line using neighbouring points as control handles.
                    // When the midpoint deviates from the straight segment, split into two segments.
                    int xNext = Math.Min(x + 1, _cols - 1);
                    Vector2 pNext = ToVec2(_points[y * _cols + xNext].Position);
                    Vector2 pPrev = ToVec2(_points[y * _cols + x - 2].Position);
                    Vector2 mid = Vector2.CatmullRom(pPrev, left, p, pNext, 0.5f);

                    if (Vector2.DistanceSquared(mid, (left + p) / 2) > 1)
                    {
                        spriteBatch.DrawLine(left, mid, color, thickness);
                        spriteBatch.DrawLine(mid, p, color, thickness);
                    }
                    else
                        spriteBatch.DrawLine(left, p, color, thickness);
                }

                if (y > 1)
                {
                    Vector2 up = ToVec2(_points[index - _cols].Position);
                    float thickness = x % 3 == 1 ? 3f : 1f;
                    spriteBatch.DrawLine(up, p, color, thickness);
                }

                if (x > 1 && y > 1)
                {
                    Vector2 up     = ToVec2(_points[index - _cols].Position);
                    Vector2 left   = ToVec2(_points[index - 1].Position);
                    Vector2 upLeft = ToVec2(_points[index - _cols - 1].Position);
                    spriteBatch.DrawLine(0.5f * (upLeft + up), 0.5f * (left + p), color, 1f);
                    spriteBatch.DrawLine(0.5f * (upLeft + left), 0.5f * (up + p), color, 1f);
                }
            }
        }
    }

    private struct PointMass(Vector3 position, float invMass)
    {
        public Vector3 Position = position;
        public Vector3 Velocity;
        public float InverseMass = invMass;
        private Vector3 _acceleration;
        private float _damping = 0.98f;

        public void ApplyForce(Vector3 force) => _acceleration += force * InverseMass;
        public void IncreaseDamping(float factor) => _damping *= factor;

        public void Update()
        {
            Velocity += _acceleration;
            Position += Velocity;
            _acceleration = Vector3.Zero;
            if (Velocity.LengthSquared() < 0.000001f)
                Velocity = Vector3.Zero;
            else
                Velocity *= _damping;
            _damping = 0.98f;
        }
    }

    private readonly struct Spring(int end1, int end2, float targetLength, float stiffness, float damping, bool toFixed = false)
    {
        private readonly int _end1 = end1;
        private readonly int _end2 = end2;
        private readonly float _stiffness = stiffness;
        private readonly float _damping = damping;
        private readonly float _targetLength = targetLength;
        private readonly bool _toFixed = toFixed;

        public void Update(PointMass[] points, PointMass[] fixedPoints)
        {
            ref var p1 = ref _toFixed ? ref fixedPoints[_end1] : ref points[_end1];
            ref var p2 = ref points[_end2];

            var x = p1.Position - p2.Position;
            float length = x.Length();

            if (length <= _targetLength) return;

            x = (x / length) * (length - _targetLength);
            var dv = p2.Velocity - p1.Velocity;
            var force = _stiffness * x - dv * _damping;

            p1.ApplyForce(-force);
            p2.ApplyForce(force);
        }
    }
}
