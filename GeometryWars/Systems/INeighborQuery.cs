using System;
using GMDCore.ECS;
using Microsoft.Xna.Framework;

namespace GeometryWars.Systems;

public interface INeighborQuery
{
    void ForEachNearbyEntity(Vector2 position, float radius, Action<Entity> visitor);
}

