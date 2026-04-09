using System;
using GeometryWars.Components.Core;
using GeometryWars.Entities;
using GeometryWars.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Components.Lifecycle;

// Plays the death burst and respawn shockwave around the owner.
public sealed class PlayRespawnEffectsComponent : Component
{
    private readonly IParticleSystem<ParticleState> _particles;
    private readonly IGridField _grid;
    private readonly Texture2D _lineParticle;
    private TransformComponent _transform;
    private RespawnStateComponent _respawnState;

    public override void OnStart(Entity owner)
    {
        if (_respawnState != null)
        {
            _respawnState.Died -= PlayDeathAtOwner;
            _respawnState.Respawned -= PlayRespawnAtOwner;
        }

        _transform = owner.Transform;
        _respawnState = owner.GetComponent<RespawnStateComponent>();
        if (_respawnState != null)
        {
            _respawnState.Died += PlayDeathAtOwner;
            _respawnState.Respawned += PlayRespawnAtOwner;
        }
    }

    public PlayRespawnEffectsComponent(IParticleSystem<ParticleState> particles, IGridField grid, Texture2D lineParticle)
    {
        _particles = particles;
        _grid = grid;
        _lineParticle = lineParticle;
    }

    public void PlayDeath(Vector2 position)
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

    public void PlayRespawn(Vector2 position)
    {
        _grid.ApplyDirectedForce(new Vector3(0, 0, 5000), new Vector3(position, 0), 50);
    }

    private void PlayDeathAtOwner() => PlayDeath(_transform.Position);

    private void PlayRespawnAtOwner() => PlayRespawn(_transform.Position);
}
