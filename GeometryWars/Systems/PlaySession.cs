using System;
using System.IO;
using GeometryWars.Entities;
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
    public EntityFactory Factory { get; }
    public EnemyDirector Spawner { get; }
    public PlayerShip Player { get; }

    public PlaySession(PlayContext context, Rectangle viewportBounds)
    {
        Score = new ScoreTracker(context.Frame, Path.Combine(AppContext.BaseDirectory, "highscore.txt"));
        var spawnResetBridge = new DeferredSpawnController();

        Entities = new EntityWorld();
        Particles = new ParticleManager<ParticleState>(
            GameSettings.Performance.MaxParticles,
            particle => ParticleState.UpdateParticle(particle, Entities.BlackHoles, context.Frame));

        Vector2 gridSpacing = new(MathF.Sqrt(viewportBounds.Width * viewportBounds.Height / (float)GameSettings.Performance.MaxGridPoints));
        Grid = new Grid(viewportBounds, gridSpacing);

        Factory = new EntityFactory(Score, Particles, Grid, Entities, spawnResetBridge, context);
        Entities.ConfigureBulletFactory(Factory.CreateBullet);
        Spawner = new EnemyDirector(Entities, Factory, context);
        spawnResetBridge.Attach(Spawner);

        Score.StartNewRun();
        Player = Factory.CreatePlayer();
        Entities.Add(Player);
    }

    public void Update()
    {
        Score.Update();
        Entities.Update();
        Spawner.Update(!Player.IsDead, () => Player.Position);
        Grid.Update();
        Particles.Update();
    }

    private sealed class DeferredSpawnController : ISpawnController
    {
        private ISpawnController _target;

        public void Attach(ISpawnController target) => _target = target;

        public void Reset()
        {
            if (_target == null)
                throw new InvalidOperationException("Spawn controller has not been attached.");

            _target.Reset();
        }
    }
}
