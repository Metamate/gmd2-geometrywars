using System;
using System.IO;
using GeometryWars.Entities;
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

    public PlaySession(Rectangle viewportBounds)
    {
        Score = new ScoreTracker(Path.Combine(AppContext.BaseDirectory, "highscore.txt"));

        EntityFactory factory = null;
        Entities = new EntityWorld(() => factory.CreateBullet());
        Particles = new ParticleManager<ParticleState>(
            GameSettings.Performance.MaxParticles,
            particle => ParticleState.UpdateParticle(particle, Entities.BlackHoles));

        Vector2 gridSpacing = new(MathF.Sqrt(viewportBounds.Width * viewportBounds.Height / (float)GameSettings.Performance.MaxGridPoints));
        Grid = new Grid(viewportBounds, gridSpacing);

        Factory = factory = new EntityFactory(Score, Particles, Grid, Entities, () => Spawner.Reset());
        Spawner = new EnemyDirector(Entities, Factory);

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
}
