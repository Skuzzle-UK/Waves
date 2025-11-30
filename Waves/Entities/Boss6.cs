using Waves.Assets;
using Waves.Assets.Audio;
using Waves.Assets.BaseAssets;
using Waves.Core.Enums;
using Waves.Core.Interfaces;
using Waves.Core.Maths;
using Waves.Entities.Builders;

namespace Waves.Entities;

/// <summary>
/// Sixth boss - alternating pattern.
/// Fires projectiles that alternate between high and low positions.
/// </summary>
public class Boss6 : BaseBoss
{
    private readonly IEntityRegistry _entityRegistry;
    private readonly IAudioManager _audioManager;

    private float _animationTimer;
    private const float AnimationSpeed = 2.2f;

    // Entrance animation state
    private const float EntranceDuration = 2.0f;
    private const float EntranceDistance = 25.0f;
    private float _entranceTimer;
    private Vector2 _spawnPosition;
    private Vector2 _finalPosition;
    private bool _entranceComplete;

    // Projectile firing state - alternating pattern
    private float _fireTimer;
    private const float FireInterval = 1.2f;
    private const float ProjectileSpeed = 85f;
    private const int BossHeight = 9;
    private bool _fireHigh = true; // Alternates between high and low

    public Boss6(IAsset asset, Vector2 homePosition, int maxHealth, IEntityRegistry entityRegistry, IAudioManager audioManager, int? seed = null)
    {
        _entityRegistry = entityRegistry ?? throw new ArgumentNullException(nameof(entityRegistry));
        _audioManager = audioManager ?? throw new ArgumentNullException(nameof(audioManager));

        Asset = asset;
        Initialize(maxHealth, homePosition, audioManager);

        _spawnPosition = homePosition;
        _finalPosition = new Vector2(homePosition.X - EntranceDistance - 5, homePosition.Y); // 5 chars further left
        _entranceTimer = 0f;
        _entranceComplete = false;

        _fireTimer = 0f;
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
            float oscillation = MathF.Sin(_animationTimer) * 2.0f;
            Position = new Vector2(HomePosition.X, HomePosition.Y + oscillation);

            _fireTimer += deltaTime;
            if (_fireTimer >= FireInterval)
            {
                FireAlternatingShot();
                _fireTimer = 0f;
            }
        }
    }

    /// <summary>
    /// Fires a projectile that alternates between high and low positions.
    /// </summary>
    private void FireAlternatingShot()
    {
        _ = _audioManager.PlayOneShot(AudioResources.SoundEffects.Tiger);

        // Alternate between high (-BossHeight/2) and low (+BossHeight/2)
        float yOffset = _fireHigh ? -BossHeight / 2f : BossHeight / 2f;
        _fireHigh = !_fireHigh; // Toggle for next shot

        Vector2 spawnPosition = new Vector2(Position.X - 3, Position.Y + yOffset);

        Projectile projectile = ProjectileBuilder.Create()
            .WithPosition(spawnPosition)
            .WithDirection(Vector2.Left)
            .WithSpeed(ProjectileSpeed)
            .WithDisplayChar('~')
            .Build();

        projectile.Layer = CollisionLayer.EnemyProjectile;
        projectile.CollidesWith = CollisionLayer.Player;

        _entityRegistry.RegisterEntity(projectile);
    }
}
