using GeometryWars.Components.Core;
using GeometryWars.Components.Visuals;
using GeometryWars.Entities;
using GeometryWars.Systems;

namespace GeometryWars.Components.Lifecycle;

// Tracks whether an entity is in its respawn window and when it can return to play.
public sealed class RespawnStateComponent : Component
{
    private readonly IScoreTracker _score;
    private int _framesUntilRespawn;
    private TransformComponent _transform;
    private SpriteComponent _sprite;

    public event System.Action Died;
    public event System.Action Respawned;

    public RespawnStateComponent(IScoreTracker score)
    {
        _score = score;
    }

    public bool IsRespawning => _framesUntilRespawn > 0;

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
        _sprite = owner.GetComponent<SpriteComponent>();
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

        Died?.Invoke();
    }

    public override void PostUpdate(Entity owner)
    {
        if (!IsRespawning)
            return;

        _framesUntilRespawn--;

        if (_framesUntilRespawn == 0 && !_score.IsGameOver)
        {
            if (_sprite != null)
                _sprite.IsActive = true;

            Respawned?.Invoke();
        }
    }
}
