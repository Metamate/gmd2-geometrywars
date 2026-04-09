using System;
using GeometryWars.Components.Core;
using GeometryWars.Components.Visuals;
using GeometryWars.Entities;
using GeometryWars.Systems;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.Lifecycle;

// Owns the full player death and respawn lifecycle:
//   1. Removes a life and starts the respawn timer.
//   2. Spawns death particles at the moment of death.
//   3. After the timer expires, applies a grid shockwave at the respawn position.
public sealed class PlayerRespawnBehaviour : Component
{
    public override ComponentUpdatePhase Phase => ComponentUpdatePhase.PostPhysics;

    private readonly IScoreTracker _score;
    private readonly IParticleSystem<ParticleState> _particles;
    private readonly IGridField _grid;
    private int _framesUntilRespawn;
    private TransformComponent _transform;
    private SpriteComponent _sprite;

    public PlayerRespawnBehaviour(IScoreTracker score, IParticleSystem<ParticleState> particles, IGridField grid)
    {
        _score = score;
        _particles = particles;
        _grid = grid;
    }

    public bool IsDead => _framesUntilRespawn > 0;

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
        _sprite    = owner.GetComponent<SpriteComponent>();
    }

    // Called by PlayerCollisionBehaviour when the ship is hit.
    public void HandleDeath()
    {
        _score.RemoveLife();

        _framesUntilRespawn = _score.IsGameOver
            ? GameSettings.Player.GameOverFrames
            : GameSettings.Player.RespawnFrames;

        if (_sprite != null)
            _sprite.IsActive = false;

        var pos = _transform.Position;
        Color yellow = new(0.8f, 0.8f, 0.4f);
        for (int i = 0; i < GameSettings.Visuals.PlayerDeathParticles; i++)
        {
            float speed = GameSettings.Visuals.DeathParticleSpeed * (1f - 1 / Random.Shared.NextFloat(1f, 10f));
            Color color = Color.Lerp(Color.White, yellow, Random.Shared.NextFloat(0, 1));
            _particles.CreateParticle(
                Art.LineParticle, pos, color,
                GameSettings.Visuals.DeathParticleLife,
                GameSettings.Visuals.DeathParticleSize,
                new ParticleState { Velocity = Random.Shared.NextVector2(speed, speed), Type = ParticleType.None, LengthMultiplier = 1 });
        }
    }

    public override void Update(Entity owner)
    {
        if (!IsDead) return;

        _framesUntilRespawn--;

        if (_framesUntilRespawn == 0 && !_score.IsGameOver)
        {
            if (_sprite != null)
                _sprite.IsActive = true;

            _grid.ApplyDirectedForce(new Vector3(0, 0, 5000), new Vector3(_transform.Position, 0), 50);
        }
    }
}
