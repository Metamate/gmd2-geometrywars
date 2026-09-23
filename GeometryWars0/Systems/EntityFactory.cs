using System;
using GeometryWars0.Definitions;
using GeometryWars0.Components.Identity;
using GeometryWars0.Components.Input;
using GeometryWars0.Components.Physics;
using GMDCore.Physics;
using GeometryWars0.Components.Visuals;
using GMDCore.ECS;
using GMDCore.Particles;
using GeometryWars0.Services;
using GeometryWars0.Utils;
using Microsoft.Xna.Framework;

namespace GeometryWars0.Systems;

// Centralizes entity recipe wiring so entities stay self-contained and
// components receive only the dependencies they actually need.
public sealed class EntityFactory
{
    private static readonly PlayerDefinition PlayerDefinition = GameplayDefinitions.Player;

    private readonly PlayContext _context;

    public EntityFactory(
        PlayContext context)
    {
        _context = context;
    }

    // Direct-control player ship with movement, twin-shot firing,
    // death/respawn flow, and ship-specific presentation effects.
    public Entity CreatePlayer()
    {
        var playerTexture = _context.Assets.GetTexture(PlayerDefinition.SpriteId);
        var player = new Entity
        {
            Transform = { Position = _context.Frame.ScreenSize / 2 }
        };

        Vector2 size = new(playerTexture.Width, playerTexture.Height);

        player.AddComponent(new Rigidbody(damping: 0f));
        player.AddComponent(new ClampToScreen(size, _context.Frame));
        player.AddComponent(new Sprite(playerTexture));
        player.AddComponent(new ApplyMovementInput(_context.Controller, PlayerDefinition.MoveSpeed));
        player.AddComponent(new GlowOverlay(_context.Assets.Glow, Color.White * PlayerDefinition.GlowOpacity));

        return player;
    }
}
