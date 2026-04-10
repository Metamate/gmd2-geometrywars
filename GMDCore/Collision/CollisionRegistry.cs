using System;
using System.Collections.Generic;
using GMDCore.Physics;
using GMDCore.ECS;
using Microsoft.Xna.Framework;

namespace GMDCore.Collision;

// Maps pairs of Collider types to their specific collision math.
public static class CollisionRegistry
{
    public delegate bool CollisionHandler(Entity a, Collider colA, Entity b, Collider colB);

    private static readonly Dictionary<(Type, Type), CollisionHandler> _handlers = new();

    static CollisionRegistry()
    {
        Register<CircleCollider, CircleCollider>(CheckCircleCircle);
        Register<BoxCollider, BoxCollider>(CheckBoxBox);
        Register<CircleCollider, BoxCollider>(CheckCircleBox);
        
        Register<BoxCollider, CircleCollider>((entityA, colA, entityB, colB) => CheckCircleBox(entityB, colB, entityA, colA));
    }

    public static void Register<TA, TB>(CollisionHandler handler) 
        where TA : Collider 
        where TB : Collider
    {
        _handlers[(typeof(TA), typeof(TB))] = handler;
    }

    public static bool Intersects(Entity a, Collider colA, Entity b, Collider colB)
    {
        if (_handlers.TryGetValue((colA.GetType(), colB.GetType()), out var handler))
        {
            return handler(a, colA, b, colB);
        }

        return false;
    }

    private static bool CheckCircleCircle(Entity a, Collider colA, Entity b, Collider colB)
    {
        var ca = (CircleCollider)colA;
        var cb = (CircleCollider)colB;
        float r = ca.Radius + cb.Radius;
        return Vector2.DistanceSquared(a.Position, b.Position) < r * r;
    }

    private static bool CheckBoxBox(Entity a, Collider colA, Entity b, Collider colB)
    {
        var ca = (BoxCollider)colA;
        var cb = (BoxCollider)colB;
        return Math.Abs(a.Position.X - b.Position.X) < (ca.Size.X + cb.Size.X) / 2 &&
               Math.Abs(a.Position.Y - b.Position.Y) < (ca.Size.Y + cb.Size.Y) / 2;
    }

    private static bool CheckCircleBox(Entity a, Collider colA, Entity b, Collider colB)
    {
        var circle = (CircleCollider)colA;
        var box = (BoxCollider)colB;

        Vector2 halfSize = box.Size / 2;
        float closestX = Math.Clamp(a.Position.X, b.Position.X - halfSize.X, b.Position.X + halfSize.X);
        float closestY = Math.Clamp(a.Position.Y, b.Position.Y - halfSize.Y, b.Position.Y + halfSize.Y);

        return Vector2.DistanceSquared(a.Position, new Vector2(closestX, closestY)) < (circle.Radius * circle.Radius);
    }
}

