using Waves.Assets;
using Waves.Assets.Audio;
using Waves.Assets.BaseAssets;
using Waves.Core.Enums;
using Waves.Core.Interfaces;
using Waves.Core.Maths;
using Waves.Entities.Builders;

namespace Waves.Entities;

/// <summary>
/// Second boss - rapid fire pattern.
/// Fires 3 quick projectiles in succession.
/// </summary>
public class Boss2 : BaseBoss
{
    private readonly IEntityRegistry _entityRegistry;
    private readonly IAudioManager _audioManager;
    private readonly Random _random;

    private float _animationTimer;
    private const float AnimationSpeed = 2.5f;

    // Entrance animation state
    private const float EntranceDuration = 2.0f;
    private const float EntranceDistance = 25.0f;
    private float _entranceTimer;
    private Vector2 _spawnPosition;
    private Vector2 _finalPosition;
    private bool _entranceComplete;

    // Projectile firing state - rapid fire pattern
    private float _fireTimer;
    private const float MinFireInterval = 0.8f;
    private const float MaxFireInterval = 1.5f;
    private float _nextFireTime;
    private const float ProjectileSpeed = 80f;
    private const int BossHeight = 9;

    public Boss2(IAsset asset, Vector2 homePosition, int maxHealth, IEntityRegistry entityRegistry, IAudioManager audioManager, int? seed = null)
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
            float oscillation = MathF.Sin(_animationTimer) * 2.5f;
            Position = new Vector2(HomePosition.X, HomePosition.Y + oscillation);

            _fireTimer += deltaTime;
            if (_fireTimer >= _nextFireTime)
            {
                FireRapidBurst();
                _fireTimer = 0f;
                _nextFireTime = GetRandomFireInterval();
            }
        }
    }

    /// <summary>
    /// Fires 3 projectiles rapidly in succession aimed at the player.
    /// </summary>
    private void FireRapidBurst()
    {
        _ = _audioManager.PlayOneShot(AudioResources.SoundEffects.Breath);

        // Fire 3 projectiles aimed at player
        for (int i = 0; i < 3; i++)
        {
            float yOffset = (float)(_random.NextDouble() * BossHeight) - (BossHeight / 2f);
            Vector2 spawnPosition = new Vector2(Position.X - 3, Position.Y + yOffset);

            // Calculate direction towards player if available, otherwise fire left
            Vector2 direction = Vector2.Left;
            if (Player != null && Player.IsActive)
            {
                direction = (Player.Position - spawnPosition).Normalized();
            }

            Projectile projectile = ProjectileBuilder.Create()
                .WithPosition(spawnPosition)
                .WithDirection(direction)
                .WithSpeed(ProjectileSpeed)
                .WithDisplayChar('*')
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
