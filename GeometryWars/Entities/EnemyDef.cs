using System;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

// Describes one enemy type. Texture is a deferred lookup so EnemyDef instances
// can be declared before Art.Load() has run.
public record EnemyDef(Func<Texture2D> GetTexture, int PointValue, float Acceleration = 0f);
