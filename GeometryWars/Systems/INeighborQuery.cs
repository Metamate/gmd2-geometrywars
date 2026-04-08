using System.Collections.Generic;
using GeometryWars.Entities;
using Microsoft.Xna.Framework;

namespace GeometryWars.Systems;

public interface INeighborQuery
{
    List<Entity> GetNearbyEntities(Vector2 position, float radius);
}
