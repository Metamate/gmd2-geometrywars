using System;
using System.IO;
using GeometryWars.Components.Lifecycle;
using GMDCore.ECS;
using GMDCore.Particles;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Systems;

// Owns the mutable state for a single Geometry Wars run.
public sealed class PlaySession
{
    public ParticleManager<ParticleState> Particles { get; }
    public Grid Grid { get; }
    public ScoreTracker Score { get; }
    public EntityWorld Entities { get; }
    public BulletSpawner BulletSpawner { get; }
    public EntityFactory Factory { get; }
    public EnemyDirector Spawner { get; }
    public Entity Player { get; }
    private readonly RespawnState _playerRespawnState;
    public bool IsPlayerRespawning => _playerRespawnState.IsRespawning;

    public PlaySession(PlayContext context, Rectangle viewportBounds)
    {
        Score = new ScoreTracker(context.Frame, Path.Combine(AppContext.BaseDirectory, "highscore.txt"));

        Entities = new EntityWorld();
        BulletSpawner = new BulletSpawner(Entities);
        Particles = new ParticleManager<ParticleState>(
            GameSettings.Performance.MaxParticles,
            particle => ParticleState.UpdateParticle(particle, Entities.BlackHoles, context.Frame));

        Vector2 gridSpacing = new(MathF.Sqrt(viewportBounds.Width * viewportBounds.Height / (float)GameSettings.Performance.MaxGridPoints));
        var gridBounds = viewportBounds;
        gridBounds.Inflate(
            (int)MathF.Ceiling(gridSpacing.X * 2f),
            (int)MathF.Ceiling(gridSpacing.Y * 2f));
        Grid = new Grid(gridBounds, gridSpacing);

        Factory = new EntityFactory(Score, Particles, Grid, Entities, BulletSpawner, context);
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
        Score.Update();
        Entities.Update();
        Spawner.Update(!IsPlayerRespawning, () => Player.Position);
        Grid.Update();
        Particles.Update();
    }

    private void HandlePlayerDied()
    {
        Entities.KillAllEnemies();
        Entities.KillAllBlackHoles();
        Spawner.Reset();
    }
}

