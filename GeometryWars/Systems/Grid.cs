using System;
using System.Collections.Generic;
using GeometryWars.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Systems;

public class Grid : IGridField
{
    private readonly Spring[] _springs;
    private readonly PointMass[,] _points;
    private readonly Vector2 _screenSize;
    private readonly int _cols;
    private readonly int _rows;

    public Grid(Rectangle size, Vector2 spacing)
    {
        _screenSize = new Vector2(size.Width, size.Height);
        _cols = (int)(size.Width / spacing.X) + 1;
        _rows = (int)(size.Height / spacing.Y) + 1;

        _points = new PointMass[_cols, _rows];

        // 1. Create the points
        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _cols; x++)
            {
                _points[x, y] = new PointMass(new Vector3(size.Left + x * spacing.X, size.Top + y * spacing.Y, 0), 1f);
            }
        }

        // 2. Link points with springs
        var springList = new List<Spring>();
        for (int y = 0; y < _rows; y++)
        {
            for (int x = 0; x < _cols; x++)
            {
                // Anchor the border and some interior points to "fixed" positions
                if (x == 0 || y == 0 || x == _cols - 1 || y == _rows - 1)
                    springList.Add(new Spring(_points[x, y], _points[x, y], 0.1f, 0.1f, true));
                else if (x % 3 == 0 && y % 3 == 0)
                    springList.Add(new Spring(_points[x, y], _points[x, y], 0.002f, 0.02f, true));

                // Mesh connections
                const float stiffness = 0.28f;
                const float damping = 0.06f;
                if (x > 0) springList.Add(new Spring(_points[x - 1, y], _points[x, y], stiffness, damping));
                if (y > 0) springList.Add(new Spring(_points[x, y - 1], _points[x, y], stiffness, damping));
            }
        }
        _springs = [.. springList];
    }

    public void Update()
    {
        foreach (var spring in _springs) spring.Update();
        foreach (var mass in _points) mass.Update();
    }

    public void ApplyDirectedForce(Vector2 force, Vector2 position, float radius)
    {
        var force3 = new Vector3(force, 0);
        var pos3 = new Vector3(position, 0);
        float rSq = radius * radius;
        foreach (var mass in _points)
        {
            float distSq = Vector3.DistanceSquared(pos3, mass.Position);
            if (distSq < rSq)
                mass.ApplyForce(10 * force3 / (10 + MathF.Sqrt(distSq)));
        }
    }

    public void ApplyImplosiveForce(float force, Vector2 position, float radius)
    {
        var pos3 = new Vector3(position, 0);
        float rSq = radius * radius;
        foreach (var mass in _points)
        {
            float distSq = Vector3.DistanceSquared(pos3, mass.Position);
            if (distSq < rSq)
            {
                mass.ApplyForce(10 * force * (pos3 - mass.Position) / (100 + distSq));
                mass.IncreaseDamping(0.6f);
            }
        }
    }

    public void ApplyExplosiveForce(float force, Vector2 position, float radius)
    {
        var pos3 = new Vector3(position, 0);
        float rSq = radius * radius;
        foreach (var mass in _points)
        {
            float distSq = Vector3.DistanceSquared(pos3, mass.Position);
            if (distSq < rSq)
            {
                mass.ApplyForce(100 * force * (mass.Position - pos3) / (10000 + distSq));
                mass.IncreaseDamping(0.6f);
            }
        }
    }

    public void ApplyDepthPulse(Vector2 position, float force, float radius)
    {
        var force3 = new Vector3(0, 0, force);
        var pos3 = new Vector3(position, 0);
        float rSq = radius * radius;
        foreach (var mass in _points)
        {
            float distSq = Vector3.DistanceSquared(pos3, mass.Position);
            if (distSq < rSq)
                mass.ApplyForce(10 * force3 / (10 + MathF.Sqrt(distSq)));
        }
    }

    private Vector2 ToVec2(Vector3 v)
    {
        float factor = (v.Z + 2000) / 2000;
        return (new Vector2(v.X, v.Y) - _screenSize / 2f) * factor + _screenSize / 2;
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
    {
        Color color = new(30, 30, 139, 85);
        for (int y = 1; y < _rows; y++)
        {
            for (int x = 1; x < _cols; x++)
            {
                Vector2 p = ToVec2(_points[x, y].Position);

                if (x > 1)
                {
                    Vector2 left = ToVec2(_points[x - 1, y].Position);
                    float thickness = y % 3 == 1 ? 3f : 1f;

                    int xNext = Math.Min(x + 1, _cols - 1);
                    Vector2 pNext = ToVec2(_points[xNext, y].Position);
                    Vector2 pPrev = ToVec2(_points[x - 2, y].Position);
                    Vector2 mid = Vector2.CatmullRom(pPrev, left, p, pNext, 0.5f);

                    if (Vector2.DistanceSquared(mid, (left + p) / 2) > 1)
                    {
                        spriteBatch.DrawLine(pixel, left, mid, color, thickness);
                        spriteBatch.DrawLine(pixel, mid, p, color, thickness);
                    }
                    else
                        spriteBatch.DrawLine(pixel, left, p, color, thickness);
                }

                if (y > 1)
                {
                    Vector2 up = ToVec2(_points[x, y - 1].Position);
                    float thickness = x % 3 == 1 ? 3f : 1f;
                    spriteBatch.DrawLine(pixel, up, p, color, thickness);
                }

                if (x > 1 && y > 1)
                {
                    Vector2 up = ToVec2(_points[x, y - 1].Position);
                    Vector2 left = ToVec2(_points[x - 1, y].Position);
                    Vector2 upLeft = ToVec2(_points[x - 1, y - 1].Position);
                    spriteBatch.DrawLine(pixel, 0.5f * (upLeft + up), 0.5f * (left + p), color, 1f);
                    spriteBatch.DrawLine(pixel, 0.5f * (upLeft + left), 0.5f * (up + p), color, 1f);
                }
            }
        }
    }

    private class PointMass(Vector3 position, float invMass)
    {
        public Vector3 Position { get; set; } = position;
        public Vector3 Velocity { get; set; }
        public float InverseMass { get; set; } = invMass;
        private Vector3 _acceleration;
        private float _damping = 0.98f;

        public void ApplyForce(Vector3 force) => _acceleration += force * InverseMass;
        public void IncreaseDamping(float factor) => _damping *= factor;

        public void Update()
        {
            Velocity += _acceleration;
            Position += Velocity;
            _acceleration = Vector3.Zero;
            if (Velocity.LengthSquared() < 0.000001f) Velocity = Vector3.Zero;
            Velocity *= _damping;
            _damping = 0.98f;
        }
    }

    private class Spring
    {
        private readonly PointMass _end1;
        private readonly PointMass _end2;
        private readonly float _targetLength;
        private readonly float _stiffness;
        private readonly float _damping;
        private readonly bool _toFixed;
        private readonly Vector3 _fixedPos;

        public Spring(PointMass end1, PointMass end2, float stiffness, float damping, bool toFixed = false)
        {
            _end1 = end1;
            _end2 = end2;
            _stiffness = stiffness;
            _damping = damping;
            _toFixed = toFixed;
            _fixedPos = end1.Position;
            _targetLength = Vector3.Distance(end1.Position, end2.Position) * 0.95f;
        }

        public void Update()
        {
            var p1Pos = _toFixed ? _fixedPos : _end1.Position;
            var p2Pos = _end2.Position;

            var x = p1Pos - p2Pos;
            float length = x.Length();

            if (length <= _targetLength) return;

            x = (x / length) * (length - _targetLength);
            var dv = _end2.Velocity - (_toFixed ? Vector3.Zero : _end1.Velocity);
            var force = _stiffness * x - dv * _damping;

            if (!_toFixed) _end1.ApplyForce(-force);
            _end2.ApplyForce(force);
        }
    }
}
