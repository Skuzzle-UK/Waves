using Waves.Assets;
using Waves.Assets.Audio;
using Waves.Assets.BaseAssets;
using Waves.Core.Enums;
using Waves.Core.Interfaces;
using Waves.Core.Maths;
using Waves.Entities.Builders;

namespace Waves.Entities;

/// <summary>
/// Fourth boss - spread shot pattern.
/// Fires 3 projectiles simultaneously at different heights.
/// </summary>
public class Boss4 : BaseBoss
{
    private readonly IEntityRegistry _entityRegistry;
    private readonly IAudioManager _audioManager;
    private readonly Random _random;

    private float _animationTimer;
    private const float AnimationSpeed = 1.8f;

    // Entrance animation state
    private const float EntranceDuration = 2.0f;
    private const float EntranceDistance = 25.0f;
    private float _entranceTimer;
    private Vector2 _spawnPosition;
    private Vector2 _finalPosition;
    private bool _entranceComplete;

    // Projectile firing state - spread shot
    private float _fireTimer;
    private const float MinFireInterval = 1.2f;
    private const float MaxFireInterval = 2.0f;
    private float _nextFireTime;
    private const float ProjectileSpeed = 70f;
    private const int BossHeight = 11;

    public Boss4(IAsset asset, Vector2 homePosition, int maxHealth, IEntityRegistry entityRegistry, IAudioManager audioManager, int? seed = null)
    {
        _entityRegistry = entityRegistry ?? throw new ArgumentNullException(nameof(entityRegistry));
        _audioManager = audioManager ?? throw new ArgumentNullException(nameof(audioManager));
        _random = seed.HasValue ? new Random(seed.Value) : new Random();

        Asset = asset;
        Initialize(maxHealth, homePosition, audioManager);

        _spawnPosition = homePosition;
        _finalPosition = new Vector2(homePosition.X - EntranceDistance, homePosition.Y);
        _entranceTimer = 0f;
        _entranceComplete = false;

        _fireTimer = 0f;
        _nextFireTime = GetRandomFireInterval();
    }

    protected override void UpdateBehavior(float deltaTime)
    {
        if (!_entranceComplete)
        {
            _entranceTimer += deltaTime;
            float progress = Math.Min(_entranceTimer / EntranceDuration, 1.0f);
            float currentX = _spawnPosition.X + (_finalPosition.X - _spawnPosition.X) * progress;
            Position = new Vector2(currentX, _spawnPosition.Y);

            if (_entranceTimer >= EntranceDuration)
            {
                _entranceComplete = true;
                HomePosition = _finalPosition;
            }
        }
        else
        {
            _animationTimer += deltaTime * AnimationSpeed;
            float oscillation = MathF.Sin(_animationTimer) * 1.5f;
            Position = new Vector2(HomePosition.X, HomePosition.Y + oscillation);

            _fireTimer += deltaTime;
            if (_fireTimer >= _nextFireTime)
            {
                FireSpreadShot();
                _fireTimer = 0f;
                _nextFireTime = GetRandomFireInterval();
            }
        }
    }

    /// <summary>
    /// Fires 3 projectiles in a spread pattern (top, middle, bottom).
    /// </summary>
    private void FireSpreadShot()
    {
        _ = _audioManager.PlayOneShot(AudioResources.SoundEffects.Shoot_003);

        // Fire at top, middle, and bottom of boss
        float[] yOffsets = { -BossHeight / 2f, 0f, BossHeight / 2f };

        foreach (float yOffset in yOffsets)
        {
            Vector2 spawnPosition = new Vector2(Position.X - 3, Position.Y + yOffset);

            Projectile projectile = ProjectileBuilder.Create()
                .WithPosition(spawnPosition)
                .WithDirection(Vector2.Left)
                .WithSpeed(ProjectileSpeed)
                .WithDisplayChar('+')
                .Build();

            projectile.Layer = CollisionLayer.EnemyProjectile;
            projectile.CollidesWith = CollisionLayer.Player;

            _entityRegistry.RegisterEntity(projectile);
        }
    }

    private float GetRandomFireInterval()
    {
        return (float)(_random.NextDouble() * (MaxFireInterval - MinFireInterval) + MinFireInterval);
    }
}
