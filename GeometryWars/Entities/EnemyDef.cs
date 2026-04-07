using System;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

// Flyweight pattern: one EnemyDef instance exists per enemy type and holds all
// shared, immutable data for that type (texture reference, point value, acceleration).
// Individual Enemy instances store only their own per-instance data (position, velocity,
// health, behaviour state). This avoids duplicating type data across every enemy object.
//
// Texture is a deferred Func<> so EnemyDef instances can be declared as static
// fields in GameSettings before Art.Load() has run. Art.Seeker / Art.Wanderer are
// themselves singleton Texture2D objects — all seekers share the same texture pointer.
public record EnemyDef(Func<Texture2D> GetTexture, int PointValue, float Acceleration = 0f);
