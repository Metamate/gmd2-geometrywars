using System;
using System.Collections.Generic;
using GeometryWars.Components;
using GeometryWars.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

public class Enemy : Entity
{
    private readonly EnemyDef _def;
    private int _timeUntilStart = GameSettings.EnemySpawnDelay;
    private readonly List<IEnumerator<int>> _behaviours = [];

    public Enemy(EnemyDef def, Vector2 position)
    {
        _def     = def;
        Image    = def.GetTexture();
        Position = position;
        Tint     = Color.Transparent;

        // Movement: decelerate each frame and stay on screen.
        AddComponent(new VelocityMover(damping: GameSettings.EnemyDamping, clampToScreen: true));

        Collider = new CircleCollider(Image.Width / 2f);
    }

    public bool IsActive => _timeUntilStart <= 0;
    public int PointValue => _def.PointValue;

    // Flyweight usage: CreateSeeker/CreateWanderer pass the shared EnemyDef
    // (declared in GameSettings) to the constructor.
    public static Enemy CreateSeeker(Vector2 position, Func<Vector2> getTargetPosition)
    {
        var def   = GameSettings.Seeker;
        var enemy = new Enemy(def, position);
        enemy.AddBehaviour(enemy.FollowPlayer(getTargetPosition, def.Acceleration));
        return enemy;
    }

    public static Enemy CreateWanderer(Vector2 position)
    {
        var def   = GameSettings.Wanderer;
        var enemy = new Enemy(def, position);
        enemy.AddBehaviour(enemy.MoveRandomly());
        return enemy;
    }

    protected override void OnUpdate()
    {
        if (_timeUntilStart <= 0)
            ApplyBehaviours();
        else
        {
            _timeUntilStart--;
            Tint = Color.White * (1 - _timeUntilStart / (float)GameSettings.EnemySpawnDelay);
        }
    }

    // Collision response:
    //   Bullet or active BlackHole → enemy dies.
    //   Another enemy              → push apart (enemies must not stack).
    //   PlayerShip                 → player handles its own side; enemy does nothing.
    //
    // Note: the IsActive guard on BlackHole prevents spawning enemies from being
    // killed by a black hole before they have fully faded in.
    public override void OnCollision(Entity other)
    {
        if (other is Bullet || (other is BlackHole && IsActive))
            WasShot();
        else if (other is Enemy e)
            HandleCollision(e);
    }

    public void HandleCollision(Enemy other)
    {
        var d = Position - other.Position;
        Velocity += 10 * d / (d.LengthSquared() + 1);
    }

    // awardPoints = false when enemies are killed as a side-effect of the player dying,
    // so the player does not score points from their own death explosion.
    public void WasShot(bool awardPoints = true)
    {
        IsExpired = true;

        float hue1 = Random.Shared.NextFloat(0, 6);
        float hue2 = (hue1 + Random.Shared.NextFloat(0, 2)) % 6f;
        Color color1 = ColorUtil.HSVToColor(hue1, 0.5f, 1);
        Color color2 = ColorUtil.HSVToColor(hue2, 0.5f, 1);
        for (int i = 0; i < GameSettings.EnemyDeathParticles; i++)
        {
            float speed = GameSettings.DeathParticleSpeed * (1f - 1 / Random.Shared.NextFloat(1f, 10f));
            var state = new ParticleState
            {
                Velocity = Random.Shared.NextVector2(speed, speed),
                Type = ParticleType.Enemy,
                LengthMultiplier = 1f
            };
            Color color = Color.Lerp(color1, color2, Random.Shared.NextFloat(0, 1));
            GameServices.Particles.CreateParticle(Art.LineParticle, Position, color, GameSettings.DeathParticleLife, GameSettings.DeathParticleSize, state);
        }

        if (awardPoints)
        {
            PlayerStatus.AddPoints(PointValue);
            PlayerStatus.IncreaseMultiplier();
        }
        GameServices.Audio.Play(Sound.Explosion, 0.5f, Random.Shared.NextFloat(-0.2f, 0.2f));
    }

    // Behaviour methods use C#'s generator (yield return) as a simple 'Coroutine'.
    // Each call to MoveNext() runs the method up to the next yield, advancing the
    // behaviour by one frame. This is a modular alternative to the State Pattern,
    // allowing AI logic to be written as a simple sequential loop.
    IEnumerable<int> FollowPlayer(Func<Vector2> getTargetPosition, float acceleration)
    {
        while (true)
        {
            Velocity += (getTargetPosition() - Position).ScaleTo(acceleration);
            if (Velocity != Vector2.Zero)
                Orientation = Velocity.ToAngle();
            yield return 0;
        }
    }

    IEnumerable<int> MoveRandomly()
    {
        float direction = Random.Shared.NextFloat(0, MathHelper.TwoPi);
        while (true)
        {
            direction += Random.Shared.NextFloat(-GameSettings.WandererTurnRate, GameSettings.WandererTurnRate);
            direction = MathHelper.WrapAngle(direction);
            for (int i = 0; i < GameSettings.WandererStepsPerTick; i++)
            {
                Velocity += MathUtil.FromPolar(direction, GameSettings.WandererVelocity);
                Orientation -= GameSettings.WandererOrientationDecay;
                var bounds = FrameContext.Viewport.Bounds;
                bounds.Inflate(-Image.Width, -Image.Height);
                if (!bounds.Contains(Position.ToPoint()))
                    direction = (FrameContext.ScreenSize / 2 - Position).ToAngle() +
                                Random.Shared.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
                yield return 0;
            }
        }
    }

    private void AddBehaviour(IEnumerable<int> behaviour) => _behaviours.Add(behaviour.GetEnumerator());

    private void ApplyBehaviours()
    {
        for (int i = 0; i < _behaviours.Count; i++)
            if (!_behaviours[i].MoveNext())
                _behaviours.RemoveAt(i--);
    }
}
