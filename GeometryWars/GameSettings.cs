namespace GeometryWars;

public static class GameSettings
{
    public static class Window
    {
        public const int Width = 1920;
        public const int Height = 1080;
    }

    public static class Performance
    {
        public const int MaxParticles = 1024 * 20;
        public const int MaxGridPoints = 1600;
        public const int MaxEntities = 200;
    }

    public static class Player
    {
        public const float Speed = 8f;
        public const int StartingLives = 1;
        public const int RespawnFrames = 120;
        public const int GameOverFrames = 120;
        public const int ExtraLifeScore = 2000;
        public const float MultiplierExpiry = 0.8f;
        public const int MaxMultiplier = 20;
    }

    public static class Bullets
    {
        public const int ShotCooldown = 6;
        public const float Speed = 20f;
        public const float Spread = 0.04f;
        public const float OffsetX = 25f;
        public const float OffsetY = 8f;
        public const float ColliderRadius = 10f;
    }

    public static class Enemy
    {
        public const int SpawnDelay = 60;
        public const float Damping = 0.8f;
        public const float WandererTurnRate = 0.1f;
        public const float WandererVelocity = 0.4f;
        public const float WandererOrientationDecay = 0.05f;
        public const int WandererStepsPerTick = 6;

        public const int   SeekerPointValue   = 2;
        public const float SeekerAcceleration = 1f;
        public const int   WandererPointValue  = 1;

        public static class Spawning
        {
            public const float ChanceStart = 60f;
            public const float ChanceMin = 20f;
            public const float ChanceDecay = 0.005f;
            public const float MinDistance = 250f;
        }
    }

    public static class Hazards
    {
        public const int BlackHoleHitpoints = 10;
        public const float BlackHoleGravityRange = 250f;
        public const float BlackHoleGravityForce = 2f;
        public const float BlackHoleGridRange = 200f;
        public const float BlackHoleSpawnChance = 200f;
        public const int MaxBlackHoles = 5;
    }

    public static class Physics
    {
        public const float BulletGridForce = 0.5f;
        public const float BulletGridRadius = 80f;
        public const float ParticleGravityForce = 10000f;
        public const float ParticleOrbitalForce = 45f;
        public const float ParticleOrbitalRange = 400f;
        public const float ParticleOrbitalDamping = 100f;
        public const float ParticleEnemyDamping = 0.94f;
        public const float ParticleDefaultDamping = 0.96f;
    }

    public static class Visuals
    {
        public const int BulletDeathParticles = 30;
        public const int PlayerDeathParticles = 1200;
        public const int EnemyDeathParticles = 120;
        public const int BlackHoleHitParticles = 150;
        public const float DeathParticleSpeed = 18f;
        public const float DeathParticleLife = 190f;
        public const float DeathParticleSize = 1.5f;
        public const float BlackHoleHitParticleMinSpeed = 8f;
        public const float BlackHoleHitParticleMaxSpeed = 16f;
    }
}
