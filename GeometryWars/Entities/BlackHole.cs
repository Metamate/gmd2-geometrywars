using System;
using GeometryWars.Components;
using GeometryWars.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

/// <summary>
/// Archetype for Black Holes.
/// Composed of visual, spatial, movement, and collision components.
/// </summary>
public class BlackHole : Entity
{
    // A black hole is "active" as soon as it exists
    public bool IsActive => true;

    public BlackHole(Vector2 position)
    {
        Image    = Art.BlackHole;
        Position = position;
        
        // Assembler: Plug in the specific behaviours of a Black Hole
        AddComponent(new MovementComponent()); // Stationary by default (damping 1, velocity 0)
        AddComponent(new GlowOverlay(Art.Glow, Color.DarkViolet * 0.4f));
        AddComponent(new GravityBehaviour(GameSettings.Hazards.BlackHoleGravityRange, GameSettings.Hazards.BlackHoleGravityForce));
        AddComponent(new SprayBehaviour(GameSettings.Hazards.BlackHoleGridRange));
        AddComponent(new BlackHoleCollisionBehaviour(GameSettings.Hazards.BlackHoleHitpoints));
        AddComponent(new CircleColliderComponent(Image.Width / 2f));
    }

    public void Kill() => IsExpired = true;

    public override void Draw(SpriteBatch spriteBatch)
    {
        // Visual pulsing effect
        float scale = 1 + 0.1f * (float)Math.Sin(10 * FrameContext.TotalSeconds);
        
        // Find orientation from MovementComponent
        float orientation = GetComponent<MovementComponent>()?.Orientation ?? 0f;
        
        spriteBatch.Draw(Image, Position, null, Tint, orientation, Size / 2f, scale, 0, 0);
        
        // Call base to draw components (like the GlowOverlay)
        base.Draw(spriteBatch);
    }
}
