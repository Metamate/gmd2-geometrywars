using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GeometryWars.Systems;

/// <summary>
/// A registry that maps pairs of Collider types to their specific collision math.
/// This allows the system to be "Open/Closed": you can add new shapes without
/// modifying the Registry or the EntityManager.
/// </summary>
public static class CollisionRegistry
{
    // A delegate for the collision math function
    public delegate bool CollisionHandler(Entity a, ColliderComponent colA, Entity b, ColliderComponent colB);

    // The lookup table: (TypeA, TypeB) -> Math Function
    private static readonly Dictionary<(Type, Type), CollisionHandler> _handlers = new();

    static CollisionRegistry()
    {
        // Initialize with the standard primitive handlers
        Register<CircleColliderComponent, CircleColliderComponent>(CheckCircleCircle);
        Register<BoxColliderComponent, BoxColliderComponent>(CheckBoxBox);
        Register<CircleColliderComponent, BoxColliderComponent>(CheckCircleBox);
        
        // Register the inverse for symmetry (Box vs Circle)
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

        // Default to no collision if we don't have math for this pair
        return false;
    }

    // --- Specific Math Implementations ---

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
