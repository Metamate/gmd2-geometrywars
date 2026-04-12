namespace GeometryWars.Definitions;

// Typed gameplay definitions
// they hold reusable content data, while the runtime behavior still lives in code.
public sealed record TwinBulletPatternDefinition(
    int CooldownFrames,
    float Speed,
    float Spread,
    float OffsetX,
    float OffsetY);

public sealed record BulletDefinition(
    SpriteId SpriteId,
    float RigidbodyDamping,
    float ColliderRadius,
    float GridForce,
    float GridRadius,
    int ExitParticleCount,
    float ExitParticleSpeed,
    float ExitParticleLifetime,
    float ExitParticleScale);

public sealed record PlayerDefinition(
    SpriteId SpriteId,
    float MoveSpeed,
    float GlowOpacity,
    float ColliderRadius,
    int DeathParticleCount,
    TwinBulletPatternDefinition PrimaryWeapon);

public sealed record EnemyShellDefinition(
    string Name,
    SpriteId SpriteId,
    int PointValue,
    float RigidbodyDamping,
    int SpawnDelayFrames,
    int DeathParticleCount);

public sealed record SeekerEnemyDefinition(
    EnemyShellDefinition Shell,
    float Acceleration);

public sealed record WanderEnemyDefinition(
    EnemyShellDefinition Shell,
    float TurnRate,
    float Velocity,
    float OrientationDecay,
    int StepsPerTick);

public sealed record BlackHoleDefinition(
    SpriteId SpriteId,
    int Hitpoints,
    float GravityRange,
    float GravityForce,
    float GridRange,
    float GlowOpacity,
    int HitParticleCount,
    float HitParticleMinSpeed,
    float HitParticleMaxSpeed);

public static class GameplayDefinitions
{
    public static PlayerDefinition Player { get; } = new(
        SpriteId.Player,
        MoveSpeed: 8f,
        GlowOpacity: 0.15f,
        ColliderRadius: 10f,
        DeathParticleCount: 1200,
        PrimaryWeapon: new TwinBulletPatternDefinition(
            CooldownFrames: 6,
            Speed: 20f,
            Spread: 0.04f,
            OffsetX: 25f,
            OffsetY: 8f));

    public static BulletDefinition Bullet { get; } = new(
        SpriteId.Bullet,
        RigidbodyDamping: 1f,
        ColliderRadius: 10f,
        GridForce: 0.5f,
        GridRadius: 80f,
        ExitParticleCount: 30,
        ExitParticleSpeed: 9f,
        ExitParticleLifetime: 50f,
        ExitParticleScale: 1f);

    public static SeekerEnemyDefinition Seeker { get; } = new(
        Shell: new EnemyShellDefinition(
            Name: "Seeker",
            SpriteId: SpriteId.Seeker,
            PointValue: 2,
            RigidbodyDamping: 0.8f,
            SpawnDelayFrames: 60,
            DeathParticleCount: 120),
        Acceleration: 1f);

    public static WanderEnemyDefinition Wanderer { get; } = new(
        Shell: new EnemyShellDefinition(
            Name: "Wanderer",
            SpriteId: SpriteId.Wanderer,
            PointValue: 1,
            RigidbodyDamping: 0.8f,
            SpawnDelayFrames: 60,
            DeathParticleCount: 120),
        TurnRate: 0.1f,
        Velocity: 0.4f,
        OrientationDecay: 0.05f,
        StepsPerTick: 6);

    public static BlackHoleDefinition BlackHole { get; } = new(
        SpriteId.BlackHole,
        Hitpoints: 10,
        GravityRange: 250f,
        GravityForce: 2f,
        GridRange: 200f,
        GlowOpacity: 0.4f,
        HitParticleCount: 150,
        HitParticleMinSpeed: 8f,
        HitParticleMaxSpeed: 16f);
}
