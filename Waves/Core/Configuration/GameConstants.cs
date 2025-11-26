using Waves.Core.Maths;

namespace Waves.Core.Configuration;

/// <summary>
/// Central configuration for all game constants and settings.
/// This class provides a single source of truth for game parameters.
/// </summary>
public static class GameConstants
{
    /// <summary>
    /// Game timing and performance settings.
    /// </summary>
    public static class Timing
    {
        /// <summary>
        /// Target frames per second for the game.
        /// </summary>
        public const int TargetFPS = 60;

        /// <summary>
        /// Fixed time step for each frame in seconds.
        /// </summary>
        public const float FixedDeltaTime = 1.0f / TargetFPS;

        /// <summary>
        /// Tick rate in milliseconds for the game loop timer.
        /// </summary>
        public const int TickRateMilliseconds = 16; // ~60 FPS

        /// <summary>
        /// Render update rate in milliseconds.
        /// </summary>
        public const int RenderUpdateMilliseconds = 16;
    }


    // TODO: I don't like these all in constants. They should be assigned inside of the class that uses them.
    /// <summary>
    /// Player entity configuration.
    /// </summary>
    public static class Player
    {
        /// <summary>
        /// Player's mass affects acceleration from forces.
        /// </summary>
        public const float Mass = 2.0f;

        /// <summary>
        /// Drag coefficient for player movement (0-1, where 1 is no drag).
        /// </summary>
        public const float DragCoefficient = 0.9f;

        /// <summary>
        /// Maximum horizontal speed for the player.
        /// </summary>
        public const float MaxSpeedHorizontal = 20.0f;

        /// <summary>
        /// Maximum vertical speed for the player.
        /// </summary>
        public const float MaxSpeedVertical = 5.0f;

        /// <summary>
        /// Force applied when player moves.
        /// </summary>
        public const float MoveForce = 300f;

        /// <summary>
        /// Base movement speed (not used in physics-based movement).
        /// </summary>
        public const float BaseSpeed = 1.0f;

        /// <summary>
        /// Default display character for the player.
        /// </summary>
        public const char DefaultCharacter = '@';

        /// <summary>
        /// Render priority for player (higher renders on top).
        /// </summary>
        public const int RenderPriority = 10;

        /// <summary>
        /// Initial health value for the player.
        /// </summary>
        public const int InitialHealth = 100;

        /// <summary>
        /// Maximum health value for the player.
        /// </summary>
        public const int MaxHealth = 100;

        /// <summary>
        /// Duration of invulnerability after taking damage in seconds.
        /// </summary>
        public const float InvulnerabilityDuration = 1.0f;

        /// <summary>
        /// Damage dealt to player when hit by terrain obstacles.
        /// </summary>
        public const int TerrainDamage = 25;
    }

    /// <summary>
    /// Projectile entity configuration.
    /// </summary>
    public static class Projectile
    {
        /// <summary>
        /// Default speed for projectiles.
        /// </summary>
        public const float DefaultSpeed = 200f;

        /// <summary>
        /// Speed for projectiles in entity definition.
        /// </summary>
        public const float EntitySpeed = 150f;

        /// <summary>
        /// Maximum distance a projectile can travel before deactivating.
        /// </summary>
        public const float MaxDistance = 1000f;

        /// <summary>
        /// Default display character for projectiles.
        /// </summary>
        public const char DefaultCharacter = '>';

        /// <summary>
        /// Alternative display character.
        /// </summary>
        public const char AlternativeCharacter = '*';

        /// <summary>
        /// Default direction for projectiles.
        /// </summary>
        public static readonly Vector2 DefaultDirection = Vector2.Right;

        /// <summary>
        /// Render priority for projectiles.
        /// </summary>
        public const int RenderPriority = 5;
    }

    /// <summary>
    /// Enemy entity configuration.
    /// </summary>
    public static class Enemy
    {
        /// <summary>
        /// Default speed for enemies (0 = stationary).
        /// </summary>
        public const float DefaultSpeed = 0f;

        /// <summary>
        /// Render priority for enemies (between projectiles and player).
        /// </summary>
        public const int RenderPriority = 8;

        /// <summary>
        /// Whether enemies should be clamped to screen bounds by default.
        /// </summary>
        public const bool ClampToBounds = true;

        /// <summary>
        /// Default health value for enemies.
        /// </summary>
        public const int DefaultHealth = 100;

        /// <summary>
        /// Score points awarded when an enemy is killed.
        /// </summary>
        public const int ScoreOnKill = 100;
    }

    /// <summary>
    /// Scoring system configuration.
    /// </summary>
    public static class Scoring
    {
        /// <summary>
        /// Time interval between score increments in seconds.
        /// </summary>
        public const float ScoreInterval = 2.0f;

        /// <summary>
        /// Points added each score interval.
        /// </summary>
        public const int ScoreIncrement = 10;

        /// <summary>
        /// Initial score value.
        /// </summary>
        public const int InitialScore = 0;
    }

    /// <summary>
    /// System update order priorities.
    /// Different ranges for different system types.
    /// </summary>
    public static class UpdateOrder
    {
        // 0-99: Input processing
        public const int InputSystem = 10;

        // 100-199: Game logic systems
        public const int ScoreSystem = 100;
        public const int ProjectileSpawner = 150;

        // 200-299: Physics and collision (reserved for future)
        public const int CollisionDetection = 250;

        // 300-399: Animation and movement
        public const int MovementSystem = 350;

        // 400-499: Rendering
        public const int RenderService = 450;
    }

    /// <summary>
    /// Input system configuration.
    /// </summary>
    public static class Input
    {
        /// <summary>
        /// Vertical movement force multiplier (as percentage of horizontal).
        /// </summary>
        public const float VerticalForceMultiplier = 0.3f;
    }

    /// <summary>
    /// Display and rendering configuration.
    /// </summary>
    public static class Display
    {
        /// <summary>
        /// Empty space character for buffer clearing.
        /// </summary>
        public const char EmptyChar = ' ';

        /// <summary>
        /// Height reduction for game grid (UI space).
        /// </summary>
        public const int GameGridHeightOffset = 3;
    }

    /// <summary>
    /// Terrain entity configuration.
    /// </summary>
    public static class Terrain
    {
        /// <summary>
        /// Minimum scrolling speed for terrain objects.
        /// </summary>
        public const float MinSpeed = 20f;

        /// <summary>
        /// Maximum scrolling speed for terrain objects.
        /// </summary>
        public const float MaxSpeed = 60f;

        /// <summary>
        /// Minimum time interval between terrain spawns in seconds.
        /// </summary>
        public const float MinSpawnInterval = 2.0f;

        /// <summary>
        /// Maximum time interval between terrain spawns in seconds.
        /// </summary>
        public const float MaxSpawnInterval = 5.0f;

        /// <summary>
        /// Render priority for terrain (behind projectiles, in front of background).
        /// </summary>
        public const int RenderPriority = 3;

        /// <summary>
        /// Default seed for terrain generation (configurable constant).
        /// </summary>
        public const int DefaultSeed = 12345;

        /// <summary>
        /// Update order for terrain spawner system.
        /// </summary>
        public const int SpawnerUpdateOrder = 125;
    }

    /// <summary>
    /// Landmass entity configuration for scrolling environmental boundaries.
    /// </summary>
    public static class Landmass
    {
        /// <summary>
        /// Base scrolling speed when AudioManager.LoopSpeed = 1.0.
        /// Actual speed = BaseScrollSpeed * AudioManager.LoopSpeed.
        /// </summary>
        public const float BaseScrollSpeed = 30f;

        /// <summary>
        /// Width of each landmass chunk in characters.
        /// </summary>
        public const float ChunkWidth = 8f;

        /// <summary>
        /// Minimum distance between gap starts (in units).
        /// </summary>
        public const float MinGapDistance = 30f;

        /// <summary>
        /// Maximum distance between gap starts (in units).
        /// </summary>
        public const float MaxGapDistance = 60f;

        /// <summary>
        /// Minimum gap width for navigation (easier gaps).
        /// </summary>
        public const float MinGapWidth = 20f;

        /// <summary>
        /// Maximum gap width for navigation (more challenging).
        /// </summary>
        public const float MaxGapWidth = 40f;

        /// <summary>
        /// Render priority for landmass chunks.
        /// Lower than terrain (3) but above background.
        /// </summary>
        public const int RenderPriority = 2;

        /// <summary>
        /// Damage dealt to player when colliding with landmass.
        /// </summary>
        public const int CollisionDamage = 15;

        /// <summary>
        /// Update order for landmass spawner system.
        /// Runs at 120 (before TerrainSpawner at 125).
        /// </summary>
        public const int SpawnerUpdateOrder = 120;
    }
}