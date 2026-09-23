using System;
using System.IO;
using GeometryWars3.Components.Lifecycle;
using GMDCore.ECS;
using GMDCore.Particles;
using GeometryWars3.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars3.Systems;

// Owns the mutable state for a single Geometry Wars run.
public sealed class PlaySession
{
    public ParticleManager<ParticleState> Particles { get; }
    public ScoreTracker Score { get; }
    public EntityWorld Entities { get; }
    public BulletSpawner BulletSpawner { get; }
    public EntityFactory Factory { get; }
    public EnemyDirector Spawner { get; }
    public Entity Player { get; }
    private readonly RespawnState _playerRespawnState;
    private bool _isShutdown;

    public bool IsPlayerRespawning => _playerRespawnState.IsRespawning;

    public PlaySession(PlayContext context, Rectangle viewportBounds)
    {
        Score = new ScoreTracker(context.Frame, Path.Combine(AppContext.BaseDirectory, "highscore.txt"));

        Entities = new EntityWorld();
        BulletSpawner = new BulletSpawner(Entities);
        Particles = new ParticleManager<ParticleState>(
            GameSettings.Performance.MaxParticles,
            particle => ParticleState.UpdateParticle(particle, Entities.BlackHoles, context.Frame));

        Factory = new EntityFactory(Score, Particles, BulletSpawner, context);
        BulletSpawner.ConfigureFactory(Factory.CreateBullet);
        Spawner = new EnemyDirector(Entities, Factory, context);

        Score.StartNewRun();
        Player = Factory.CreatePlayer();
        _playerRespawnState = Player.RequireComponent<RespawnState>();
        _playerRespawnState.Died += HandlePlayerDied;
        Entities.Add(Player);
    }

    public void Update()
    {
        if (_isShutdown)
            throw new InvalidOperationException("PlaySession cannot update after shutdown.");

        UpdateSessionRules();
        UpdateWorld();
        UpdateSpawning();
        UpdateVisualSystems();
    }

    private void HandlePlayerDied()
    {
        Entities.KillAllEnemies();
        Entities.KillAllBlackHoles();
        Spawner.Reset();
    }

    public void Shutdown()
    {
        if (_isShutdown)
            return;

        _playerRespawnState.Died -= HandlePlayerDied;
        Entities.Clear();
        BulletSpawner.Shutdown();
        Particles.Clear();
        _isShutdown = true;
    }

    private void UpdateSessionRules()
    {
        Score.Update();
        Spawner.Update(!IsPlayerRespawning, () => Player.Position);
    }

    private void UpdateWorld()
    {
        Entities.Update();
    }

    private void UpdateSpawning()
    {
        Spawner.Update(!IsPlayerRespawning, () => Player.Position);
    }

    private void UpdateVisualSystems()
    {
        Particles.Update();
    }
}
