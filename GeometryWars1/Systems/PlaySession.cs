using System;
using System.IO;
using GMDCore.ECS;
using GMDCore.Particles;
using GeometryWars1.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars1.Systems;

// Owns the mutable state for a single Geometry Wars run.
public sealed class PlaySession
{
    public EntityWorld Entities { get; }
    public BulletSpawner BulletSpawner { get; }
    public EntityFactory Factory { get; }
    public Entity Player { get; }
    private bool _isShutdown;

    public PlaySession(PlayContext context, Rectangle viewportBounds)
    {
        Entities = new EntityWorld();
        BulletSpawner = new BulletSpawner(Entities);

        Factory = new EntityFactory(BulletSpawner, context);
        BulletSpawner.ConfigureFactory(Factory.CreateBullet);

        Player = Factory.CreatePlayer();
        Entities.Add(Player);
    }

    public void Update()
    {
        if (_isShutdown)
            throw new InvalidOperationException("PlaySession cannot update after shutdown.");

        UpdateWorld();
    }

    public void Shutdown()
    {
        if (_isShutdown)
            return;

        Entities.Clear();
        BulletSpawner.Shutdown();
        _isShutdown = true;
    }

    private void UpdateWorld()
    {
        Entities.Update();
    }
}
