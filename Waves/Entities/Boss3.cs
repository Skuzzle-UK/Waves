using Waves.Assets;
using Waves.Assets.Audio;
using Waves.Assets.BaseAssets;
using Waves.Core.Enums;
using Waves.Core.Interfaces;
using Waves.Core.Maths;
using Waves.Entities.Builders;

namespace Waves.Entities;

/// <summary>
/// Third boss - frequent burst pattern.
/// Fires single projectiles more frequently.
/// </summary>
public class Boss3 : BaseBoss
{
    private readonly IEntityRegistry _entityRegistry;
    private readonly IAudioManager _audioManager;
    private readonly Random _random;

    private float _animationTimer;
    private const float AnimationSpeed = 3.0f;

    // Entrance animation state
    private const float EntranceDuration = 2.0f;
    private const float EntranceDistance = 25.0f;
    private float _entranceTimer;
    private Vector2 _spawnPosition;
    private Vector2 _finalPosition;
    private bool _entranceComplete;

    // Projectile firing state - frequent single shots
    private float _fireTimer;
    private const float MinFireInterval = 0.5f;
    private const float MaxFireInterval = 1.0f;
    private float _nextFireTime;
    private const float ProjectileSpeed = 75f;
    private const int BossHeight = 13;

    public Boss3(IAsset asset, Vector2 homePosition, int maxHealth, IEntityRegistry entityRegistry, IAudioManager audioManager, int? seed = null)
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
            float oscillation = MathF.Sin(_animationTimer) * 3.0f;
            Position = new Vector2(HomePosition.X, HomePosition.Y + oscillation);

            _fireTimer += deltaTime;
            if (_fireTimer >= _nextFireTime)
            {
                FireSingleShot();
                _fireTimer = 0f;
                _nextFireTime = GetRandomFireInterval();
            }
        }
    }

    /// <summary>
    /// Fires a single fast projectile.
    /// </summary>
    private void FireSingleShot()
    {
        _ = _audioManager.PlayOneShot(AudioResources.SoundEffects.Horn);

        float yOffset = (float)(_random.NextDouble() * BossHeight) - (BossHeight / 2f);
        Vector2 spawnPosition = new Vector2(Position.X - 3, Position.Y + yOffset);

        Projectile projectile = ProjectileBuilder.Create()
            .WithPosition(spawnPosition)
            .WithDirection(Vector2.Left)
            .WithSpeed(ProjectileSpeed)
            .WithDisplayChar('◆')
            .Build();

        projectile.Layer = CollisionLayer.EnemyProjectile;
        projectile.CollidesWith = CollisionLayer.Player;

        _entityRegistry.RegisterEntity(projectile);
    }

    private float GetRandomFireInterval()
    {
        return (float)(_random.NextDouble() * (MaxFireInterval - MinFireInterval) + MinFireInterval);
    }
}
