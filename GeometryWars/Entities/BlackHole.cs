using GeometryWars.Components;
using Microsoft.Xna.Framework;

namespace GeometryWars;

public class BlackHole : Entity
{
    public BlackHole(Vector2 position)
    {
        Transform.Position = position;
        
        AddComponent(new RigidbodyComponent()); 
        AddComponent(new SpriteComponent(Art.BlackHole));
        AddComponent(new GlowOverlay(Art.Glow, Color.DarkViolet * 0.4f));
        AddComponent(new GravityBehaviour(GameSettings.Hazards.BlackHoleGravityRange, GameSettings.Hazards.BlackHoleGravityForce));
        AddComponent(new SprayBehaviour(GameSettings.Hazards.BlackHoleGridRange));
        AddComponent(new BlackHoleCollisionBehaviour(GameSettings.Hazards.BlackHoleHitpoints));
        AddComponent(new CircleColliderComponent(Art.BlackHole.Width / 2f));
    }

    public void Kill() => IsExpired = true;
}
