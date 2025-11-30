using Waves.Assets;
using Waves.Assets.Audio;
using Waves.Assets.BaseAssets;
using Waves.Core.Enums;
using Waves.Core.Interfaces;
using Waves.Core.Maths;
using Waves.Entities.Builders;

namespace Waves.Entities;

/// <summary>
/// First boss encounter - a powerful stationary enemy with light animations.
/// Appears at the end of level 1 after 3 minutes of gameplay.
/// Fires projectiles at the player from various heights.
/// </summary>
public class Boss1 : BaseBoss
{
    private readonly IEntityRegistry _entityRegistry;
    private readonly IAudioManager _audioManager;
    private readonly Random _random;

    private float _animationTimer;
    private const float AnimationSpeed = 2.0f; // Oscillation frequency

    // Entrance animation state
    private const float EntranceDuration = 2.0f; // 2 seconds entrance
    private const float EntranceDistance = 25.0f; // Move units left
    private float _entranceTimer;
    private Vector2 _spawnPosition;
    private Vector2 _finalPosition;
    private bool _entranceComplete;

    // Projectile firing state
    private float _fireTimer;
    private const float MinFireInterval = 1.0f; // Minimum time between shots
    private const float MaxFireInterval = 2.5f; // Maximum time between shots
    private float _nextFireTime;
    private const float ProjectileSpeed = 70f;
    private const int BossHeight = 5; // Height of the boss visual

    public Boss1(IAsset asset, Vector2 homePosition, int maxHealth, IEntityRegistry entityRegistry, IAudioManager audioManager, int? seed = null)
    {
        _entityRegistry = entityRegistry ?? throw new ArgumentNullException(nameof(entityRegistry));
        _audioManager = audioManager ?? throw new ArgumentNullException(nameof(audioManager));
        _random = seed.HasValue ? new Random(seed.Value) : new Random();

        Asset = asset;
        Initialize(maxHealth, homePosition, audioManager);

        // Set up entrance animation positions
        _spawnPosition = homePosition;
        _finalPosition = new Vector2(homePosition.X - EntranceDistance, homePosition.Y);
        _entranceTimer = 0f;
        _entranceComplete = false;

        // Set up projectile firing
        _fireTimer = 0f;
        _nextFireTime = GetRandomFireInterval();
    }

    /// <summary>
    /// Boss1 behavior: entrance animation followed by slight vertical oscillation and projectile firing.
    /// </summary>
    protected override void UpdateBehavior(float deltaTime)
    {
        // Entrance phase: move in from the right
        if (!_entranceComplete)
        {
            _entranceTimer += deltaTime;

            // Calculate progress (0.0 to 1.0)
            float progress = Math.Min(_entranceTimer / EntranceDuration, 1.0f);

            // Linear interpolation from spawn to final position
            float currentX = _spawnPosition.X + (_finalPosition.X - _spawnPosition.X) * progress;
            Position = new Vector2(currentX, _spawnPosition.Y);

            // Check if entrance is complete
            if (_entranceTimer >= EntranceDuration)
            {
                _entranceComplete = true;
                // Update HomePosition to the final position for oscillation
                HomePosition = _finalPosition;
            }
        }
        // Normal behavior phase: gentle oscillation and firing
        else
        {
            _animationTimer += deltaTime * AnimationSpeed;

            // Gentle vertical oscillation (±2 units)
            float oscillation = MathF.Sin(_animationTimer) * 2.0f;

            Position = new Vector2(HomePosition.X, HomePosition.Y + oscillation);

            // Handle projectile firing
            _fireTimer += deltaTime;
            if (_fireTimer >= _nextFireTime)
            {
                FireProjectiles();
                _fireTimer = 0f;
                _nextFireTime = GetRandomFireInterval();
            }
        }
    }

    /// <summary>
    /// Fires 1 or 2 projectiles at random heights from the boss.
    /// </summary>
    private void FireProjectiles()
    {
        // Play firing sound effect once (not per projectile)
        _ = _audioManager.PlayOneShot(AudioResources.SoundEffects.Shoot_003);

        // 50% chance to fire 2 projectiles, 50% chance to fire 1
        int projectileCount = _random.Next(2) == 0 ? 1 : 2;

        for (int i = 0; i < projectileCount; i++)
        {
            // Random Y offset within the boss's height (-2 to +2 from center)
            float yOffset = (float)(_random.NextDouble() * BossHeight) - (BossHeight / 2f);
            Vector2 spawnPosition = new Vector2(Position.X - 3, Position.Y + yOffset);

            // Create animated projectile moving left
            Projectile projectile = ProjectileBuilder.Create()
                .WithPosition(spawnPosition)
                .WithDirection(Vector2.Left) // Move towards player (left)
                .WithSpeed(ProjectileSpeed)
                .WithAsset(ProjectileAssets.SpinningProjectile)
                .Build();

            // Set collision properties for enemy projectile
            projectile.Layer = CollisionLayer.EnemyProjectile;
            projectile.CollidesWith = CollisionLayer.Player;

            // Register the projectile
            _entityRegistry.RegisterEntity(projectile);
        }
    }

    /// <summary>
    /// Gets a random fire interval between min and max.
    /// </summary>
    private float GetRandomFireInterval()
    {
        return (float)(_random.NextDouble() * (MaxFireInterval - MinFireInterval) + MinFireInterval);
    }
}
