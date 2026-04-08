using System;
using System.Collections.Generic;
using GeometryWars.Components;
using Microsoft.Xna.Framework;

namespace GeometryWars.Systems;

public static class CollisionRegistry
{
    public delegate bool CollisionHandler(Entity a, ColliderComponent colA, Entity b, ColliderComponent colB);

    private static readonly Dictionary<(Type, Type), CollisionHandler> _handlers = new();

    static CollisionRegistry()
    {
        Register<CircleColliderComponent, CircleColliderComponent>(CheckCircleCircle);
        Register<BoxColliderComponent, BoxColliderComponent>(CheckBoxBox);
        Register<CircleColliderComponent, BoxColliderComponent>(CheckCircleBox);
        
        Register<BoxColliderComponent, CircleColliderComponent>((eb, cb, ec, cc) => CheckCircleBox(ec, cc, eb, cb));
    }

    public static void Register<TA, TB>(CollisionHandler handler) 
        where TA : ColliderComponent 
        where TB : ColliderComponent
    {
        _handlers[(typeof(TA), typeof(TB))] = handler;
    }

    public static bool Intersects(Entity a, ColliderComponent colA, Entity b, ColliderComponent colB)
    {
        if (_handlers.TryGetValue((colA.GetType(), colB.GetType()), out var handler))
        {
            return handler(a, colA, b, colB);
        }

        return false;
    }

    private static bool CheckCircleCircle(Entity a, ColliderComponent colA, Entity b, ColliderComponent colB)
    {
        var ca = (CircleColliderComponent)colA;
        var cb = (CircleColliderComponent)colB;
        float r = ca.Radius + cb.Radius;
        return Vector2.DistanceSquared(a.Position, b.Position) < r * r;
    }

    private static bool CheckBoxBox(Entity a, ColliderComponent colA, Entity b, ColliderComponent colB)
    {
        var ca = (BoxColliderComponent)colA;
        var cb = (BoxColliderComponent)colB;
        return Math.Abs(a.Position.X - b.Position.X) < (ca.Size.X + cb.Size.X) / 2 &&
               Math.Abs(a.Position.Y - b.Position.Y) < (ca.Size.Y + cb.Size.Y) / 2;
    }

    private static bool CheckCircleBox(Entity a, ColliderComponent colA, Entity b, ColliderComponent colB)
    {
        var circle = (CircleColliderComponent)colA;
        var box = (BoxColliderComponent)colB;

        Vector2 halfSize = box.Size / 2;
        float closestX = Math.Clamp(a.Position.X, b.Position.X - halfSize.X, b.Position.X + halfSize.X);
        float closestY = Math.Clamp(a.Position.Y, b.Position.Y - halfSize.Y, b.Position.Y + halfSize.Y);

        return Vector2.DistanceSquared(a.Position, new Vector2(closestX, closestY)) < (circle.Radius * circle.Radius);
    }
}
