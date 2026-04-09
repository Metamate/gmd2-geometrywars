using System;
using GeometryWars.Components.Core;
using GeometryWars.Entities;
using GeometryWars.Systems;
using GeometryWars.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Components.Lifecycle;

// Plays the death burst and respawn shockwave around the owner.
public sealed class PlayRespawnEffects : Component
{
    private readonly IParticleSystem<ParticleState> _particles;
    private readonly IGridField _grid;
    private readonly Texture2D _lineParticle;
    private Transform _transform;
    private RespawnState _respawnState;

    public override void OnStart(Entity owner)
    {
        if (_respawnState != null)
        {
            _respawnState.Died -= PlayDeathAtOwner;
            _respawnState.Respawned -= PlayRespawnAtOwner;
        }

        _transform = owner.Transform;
        _respawnState = owner.GetComponent<RespawnState>();
        if (_respawnState != null)
        {
            _respawnState.Died += PlayDeathAtOwner;
            _respawnState.Respawned += PlayRespawnAtOwner;
        }
    }

    public PlayRespawnEffects(IParticleSystem<ParticleState> particles, IGridField grid, Texture2D lineParticle)
    {
        _particles = particles;
        _grid = grid;
        _lineParticle = lineParticle;
    }

    private void PlayDeath(Vector2 position)
    {
        Color yellow = new(0.8f, 0.8f, 0.4f);
        for (int i = 0; i < GameSettings.Visuals.PlayerDeathParticles; i++)
        {
            float speed = GameSettings.Visuals.DeathParticleSpeed * (1f - 1 / Random.Shared.NextFloat(1f, 10f));
            Color color = Color.Lerp(Color.White, yellow, Random.Shared.NextFloat(0, 1));
            _particles.CreateParticle(
                _lineParticle, position, color,
                GameSettings.Visuals.DeathParticleLife,
                GameSettings.Visuals.DeathParticleSize,
                new ParticleState { Velocity = Random.Shared.NextVector2(speed, speed), Type = ParticleType.None, LengthMultiplier = 1 });
        }
    }

    private void PlayRespawn(Vector2 position)
    {
        _grid.ApplyDepthPulse(position, 5000, 50);
    }

    private void PlayDeathAtOwner() => PlayDeath(_transform.Position);

    private void PlayRespawnAtOwner() => PlayRespawn(_transform.Position);
}
