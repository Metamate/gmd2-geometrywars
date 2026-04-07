namespace GeometryWars;

public static class GameSettings
{
    // ── Enemy Definitions ─────────────────────────────────────────────────────
    // Declared here so all per-type config lives in one place.
    // Texture is deferred via Func<> so these can be created before Art.Load().
    public static readonly EnemyDef Seeker   = new(() => Art.Seeker,   SeekerPoints,   SeekerAcceleration);
    public static readonly EnemyDef Wanderer = new(() => Art.Wanderer, WandererPoints, Acceleration: 0f);

    // ── Window ───────────────────────────────────────────────────────────────
    public const int ScreenWidth  = 1920;
    public const int ScreenHeight = 1080;

    // ── Particles & Grid ─────────────────────────────────────────────────────
    public const int MaxParticles  = 1024 * 20;
    public const int MaxGridPoints = 1600;

    // ── Player ───────────────────────────────────────────────────────────────
    public const float PlayerSpeed          = 8f;
    public const int   PlayerShotCooldown   = 6;       // frames between shots
    public const float PlayerBulletSpeed    = 20f;
    public const float PlayerBulletSpread   = 0.04f;   // applied twice per shot (±0.08f total)
    public const float PlayerBulletOffsetX  = 25f;
    public const float PlayerBulletOffsetY  = 8f;
    public const float PlayerColliderRadius = 10f;
    public const int   PlayerRespawnFrames  = 120;
    public const int   PlayerGameOverFrames = 300;

    // ── Player Status ─────────────────────────────────────────────────────────
    public const int   PlayerStartingLives    = 4;
    public const int   PlayerExtraLifeScore   = 2000;
    public const float PlayerMultiplierExpiry = 0.8f;  // seconds before multiplier resets
    public const int   PlayerMaxMultiplier    = 20;

    // ── Enemy ─────────────────────────────────────────────────────────────────
    public const int   EnemySpawnDelay           = 60;    // frames before enemy becomes active
    public const float EnemyDamping              = 0.8f;
    public const float SeekerAcceleration        = 1f;
    public const int   SeekerPoints              = 2;
    public const float WandererTurnRate          = 0.1f;
    public const float WandererVelocity          = 0.4f;
    public const float WandererOrientationDecay  = 0.05f;
    public const int   WandererStepsPerTick      = 6;
    public const int   WandererPoints            = 1;

    // ── Enemy Spawner ─────────────────────────────────────────────────────────
    public const float EnemySpawnChanceStart  = 60f;   // inverse: 1-in-N chance per frame
    public const float EnemySpawnChanceMin    = 20f;
    public const float EnemySpawnChanceDecay  = 0.005f;
    public const float BlackHoleSpawnChance   = 600f;
    public const int   MaxActiveEntities      = 200;
    public const int   MaxBlackHoles          = 2;
    public const float SpawnMinPlayerDistance = 250f;

    // ── Black Hole ────────────────────────────────────────────────────────────
    public const int   BlackHoleHitpoints    = 10;
    public const float BlackHoleGravityRange = 250f;
    public const float BlackHoleGravityForce = 2f;
    public const float BlackHoleGridRange    = 200f;

    // ── Bullet ────────────────────────────────────────────────────────────────
    public const float BulletColliderRadius = 8f;
    public const float BulletGridForce      = 0.5f;
    public const float BulletGridRadius     = 80f;
    public const int   BulletDeathParticles = 30;

    // ── Particle Physics ──────────────────────────────────────────────────────
    public const float ParticleGravityForce    = 10000f;
    public const float ParticleOrbitalForce    = 45f;
    public const float ParticleOrbitalRange    = 400f;
    public const float ParticleOrbitalDamping  = 100f;
    public const float ParticleEnemyDamping    = 0.94f;
    public const float ParticleDefaultDamping  = 0.96f;

    // ── Death Explosions ──────────────────────────────────────────────────────
    public const int   PlayerDeathParticles = 1200;
    public const int   EnemyDeathParticles  = 120;
    public const float DeathParticleSpeed   = 18f;
    public const float DeathParticleLife    = 190f;
    public const float DeathParticleSize    = 1.5f;

    // ── Black Hole Hit Effect ─────────────────────────────────────────────────
    public const int   BlackHoleHitParticles  = 150;
    public const float BlackHoleHitParticleMinSpeed = 8f;
    public const float BlackHoleHitParticleMaxSpeed = 16f;
    public const float BlackHoleHitParticleLife     = 90f;
    public const float BlackHoleHitParticleSize     = 1.5f;
}
