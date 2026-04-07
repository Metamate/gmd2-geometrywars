using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

// Spring-mass cloth simulation used as a visual background distortion effect.
//
// The grid is made of two building blocks:
//
//   PointMass — a node with a 3D position and velocity. InverseMass = 0 means the
//   point is fixed (infinite mass, cannot be moved by forces). InverseMass = 1 gives
//   normal mass. Acceleration accumulates applied forces (F = m*a → a = F / m = F * invMass)
//   and is consumed in Update() using semi-implicit Euler integration:
//       velocity += acceleration
//       position += velocity
//       velocity *= damping      (energy loss each frame)
//
//   Spring — connects two PointMasses and enforces a target length using Hooke's law.
//   These springs only pull (not push): if the current length is less than TargetLength,
//   no force is applied. Force = stiffness * extension − damping * relative_velocity.
//   Each spring applies equal and opposite forces to its two endpoints.
//
// Grid topology:
//   Border points  → strongly anchored to fixed positions (stiffness 0.1) so the
//                    edges stay roughly in place no matter what happens in the middle.
//   1/9 interior points → loosely anchored (stiffness 0.002) to gently pull the
//                    grid back to rest over time.
//   All neighbouring points → connected by mesh springs (stiffness 0.28) that
//                    transmit disturbance waves across the grid.
//
// External forces:
//   ApplyDirectedForce   — pushes nearby points in a given direction (player respawn).
//   ApplyExplosiveForce  — pushes points outward from an origin (bullet impact).
//   ApplyImplosiveForce  — pulls points toward an origin (black hole gravity).
//
// Rendering:
//   ToVec2() applies a simple perspective projection: points with Z < 0 (pushed "into"
//   the screen) appear to shrink toward the centre; Z > 0 expands outward.
//   Catmull-Rom interpolation smooths sharp bends between grid line segments.
public class GridOld
{
    Spring[] springs;
    PointMass[,] points;
    Vector2 screenSize;

    public GridOld(Rectangle size, Vector2 spacing)
    {
        screenSize = new Vector2(size.Width, size.Height);
        var springList = new List<Spring>();
        int numColumns = (int)(size.Width / spacing.X) + 1;
        int numRows = (int)(size.Height / spacing.Y) + 1;
        points = new PointMass[numColumns, numRows];
        // these fixed points will be used to anchor the grid to fixed positions on the screen 
        PointMass[,] fixedPoints = new PointMass[numColumns, numRows];
        // create the point masses 
        int column = 0, row = 0;
        for (float y = size.Top; y <= size.Bottom; y += spacing.Y)
        {
            for (float x = size.Left; x <= size.Right; x += spacing.X)
            {
                points[column, row] = new PointMass(new Vector3(x, y, 0), 1);
                fixedPoints[column, row] = new PointMass(new Vector3(x, y, 0), 0);
                column++;
            }
            row++;
            column = 0;
        }
        // link the point masses with springs 
        for (int y = 0; y < numRows; y++)
            for (int x = 0; x < numColumns; x++)
            {
                if (x == 0 || y == 0 || x == numColumns - 1 || y == numRows - 1)    // anchor the border of the grid 
                    springList.Add(new Spring(fixedPoints[x, y], points[x, y], 0.1f, 0.1f));
                else if (x % 3 == 0 && y % 3 == 0)                                  // loosely anchor 1/9th of the point masses 
                    springList.Add(new Spring(fixedPoints[x, y], points[x, y], 0.002f, 0.02f));
                const float stiffness = 0.28f;
                const float damping = 0.06f;
                if (x > 0)
                    springList.Add(new Spring(points[x - 1, y], points[x, y], stiffness, damping));
                if (y > 0)
                    springList.Add(new Spring(points[x, y - 1], points[x, y], stiffness, damping));
            }
        springs = springList.ToArray();
    }

    public void Update()
    {
        foreach (var spring in springs)
            spring.Update();
        foreach (var mass in points)
            mass.Update();
    }
    public void ApplyDirectedForce(Vector2 force, Vector2 position, float radius)
    {
        ApplyDirectedForce(new Vector3(force, 0), new Vector3(position, 0), radius);
    }
    public void ApplyDirectedForce(Vector3 force, Vector3 position, float radius)
    {
        foreach (var mass in points)
            if (Vector3.DistanceSquared(position, mass.Position) < radius * radius)
                mass.ApplyForce(10 * force / (10 + Vector3.Distance(position, mass.Position)));
    }
    public void ApplyImplosiveForce(float force, Vector2 position, float radius)
    {
        ApplyImplosiveForce(force, new Vector3(position, 0), radius);
    }
    public void ApplyImplosiveForce(float force, Vector3 position, float radius)
    {
        foreach (var mass in points)
        {
            float dist2 = Vector3.DistanceSquared(position, mass.Position);
            if (dist2 < radius * radius)
            {
                mass.ApplyForce(10 * force * (position - mass.Position) / (100 + dist2));
                mass.IncreaseDamping(0.6f);
            }
        }
    }
    public void ApplyExplosiveForce(float force, Vector2 position, float radius)
    {
        ApplyExplosiveForce(force, new Vector3(position, 0), radius);
    }
    public void ApplyExplosiveForce(float force, Vector3 position, float radius)
    {
        foreach (var mass in points)
        {
            float dist2 = Vector3.DistanceSquared(position, mass.Position);
            if (dist2 < radius * radius)
            {
                mass.ApplyForce(100 * force * (mass.Position - position) / (10000 + dist2));
                mass.IncreaseDamping(0.6f);
            }
        }
    }

    public Vector2 ToVec2(Vector3 v)
    {
        // do a perspective projection 
        float factor = (v.Z + 2000) / 2000;
        return (new Vector2(v.X, v.Y) - screenSize / 2f) * factor + screenSize / 2;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        int width = points.GetLength(0);
        int height = points.GetLength(1);
        Color color = new(30, 30, 139, 85);   // dark blue 
        for (int y = 1; y < height; y++)
        {
            for (int x = 1; x < width; x++)
            {
                Vector2 left = new(), up = new(); Vector2 p = ToVec2(points[x, y].Position); if (x > 1)
                {
                    left = ToVec2(points[x - 1, y].Position);
                    float thickness = y % 3 == 1 ? 3f : 1f;
                    // use Catmull-Rom interpolation to help smooth bends in the grid 
                    int clampedX = Math.Min(x + 1, width - 1);
                    Vector2 mid = Vector2.CatmullRom(ToVec2(points[x - 2, y].Position), left, p, ToVec2(points[clampedX, y].Position), 0.5f);
                    // If the grid is very straight here, draw a single straight line. Otherwise, draw lines to our 
                    // new interpolated midpoint 
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
                    up = ToVec2(points[x, y - 1].Position);
                    float thickness = x % 3 == 1 ? 3f : 1f;
                    spriteBatch.DrawLine(up, p, color, thickness);
                }
                if (x > 1 && y > 1)
                {
                    Vector2 upLeft = ToVec2(points[x - 1, y - 1].Position);
                    spriteBatch.DrawLine(0.5f * (upLeft + up), 0.5f * (left + p), color, 1f);   // vertical line 
                    spriteBatch.DrawLine(0.5f * (upLeft + left), 0.5f * (up + p), color, 1f);   // horizontal line 
                }
            }

        }

    }

    private class PointMass(Vector3 position, float invMass)
    {
        public Vector3 Position = position;
        public Vector3 Velocity;
        public float InverseMass = invMass;
        private Vector3 acceleration;
        private float damping = 0.98f;

        public void ApplyForce(Vector3 force)
        {
            acceleration += force * InverseMass;
        }
        public void IncreaseDamping(float factor)
        {
            damping *= factor;
        }
        public void Update()
        {
            Velocity += acceleration;
            Position += Velocity;
            acceleration = Vector3.Zero;
            if (Velocity.LengthSquared() < 0.001f * 0.001f)
                Velocity = Vector3.Zero;
            Velocity *= damping;
            damping = 0.98f;
        }
    }

    private struct Spring(PointMass end1, PointMass end2, float stiffness, float damping)
    {
        public PointMass End1 = end1;
        public PointMass End2 = end2;
        public float TargetLength = Vector3.Distance(end1.Position, end2.Position) * 0.95f;
        public float Stiffness = stiffness;
        public float Damping = damping;

        public void Update()
        {
            var x = End1.Position - End2.Position;
            float length = x.Length();
            // these springs can only pull, not push 
            if (length <= TargetLength)
                return;
            x = (x / length) * (length - TargetLength);
            var dv = End2.Velocity - End1.Velocity;
            var force = Stiffness * x - dv * Damping;
            End1.ApplyForce(-force);
            End2.ApplyForce(force);
        }
    }

}