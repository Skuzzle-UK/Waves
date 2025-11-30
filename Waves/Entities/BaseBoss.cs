using Waves.Assets.Audio;
using Waves.Assets.BaseAssets;
using Waves.Core.Configuration;
using Waves.Core.Enums;
using Waves.Core.Interfaces;
using Waves.Core.Maths;

namespace Waves.Entities;

/// <summary>
/// Abstract base class for boss entities.
/// Bosses are powerful enemies that appear at the end of levels with custom behavior patterns.
/// </summary>
public abstract class BaseBoss : BaseEntity
{
    /// <summary>
    /// Maximum health points for this boss.
    /// </summary>
    public int MaxHealth { get; protected set; }

    /// <summary>
    /// Current health points for this boss.
    /// </summary>
    public int CurrentHealth { get; protected set; }

    /// <summary>
    /// Whether this boss has been defeated.
    /// </summary>
    public bool IsDefeated => CurrentHealth <= 0;

    /// <summary>
    /// Event fired when the boss is defeated.
    /// </summary>
    public event EventHandler? OnDefeated;

    /// <summary>
    /// Event fired when the boss takes damage.
    /// </summary>
    public event EventHandler<int>? OnDamaged;

    /// <summary>
    /// Time elapsed since the boss spawned (for behavior patterns).
    /// </summary>
    protected float ElapsedTime { get; private set; }

    /// <summary>
    /// Reference position for the boss (typically right side of screen).
    /// </summary>
    protected Vector2 HomePosition { get; set; }

    /// <summary>
    /// Audio manager for playing sound effects.
    /// </summary>
    protected IAudioManager? AudioManager { get; private set; }

    /// <summary>
    /// Timer for how long the boss should flash after taking damage.
    /// </summary>
    private float _damageFlashTimer = 0f;

    /// <summary>
    /// Timer for toggling the flash color.
    /// </summary>
    private float _flashTimer = 0f;

    /// <summary>
    /// Whether to show red flash (toggles on/off).
    /// </summary>
    private bool _showRedFlash = false;

    /// <summary>
    /// Duration of the damage flash effect.
    /// </summary>
    private const float DamageFlashDuration = 0.2f;

    /// <summary>
    /// Interval for toggling flash color (same as player).
    /// </summary>
    private const float FlashInterval = 0.2f;

    protected BaseBoss()
    {
        // Configure boss properties
        Layer = CollisionLayer.Enemy;
        CollidesWith = CollisionLayer.Player | CollisionLayer.PlayerProjectile;
        ClampToBounds = true; // Keep boss on screen
        RenderPriority = GameConstants.Enemy.RenderPriority + 1; // Render above regular enemies
        Speed = 0f; // Bosses typically don't scroll
        IsActive = true;
    }

    /// <summary>
    /// Initializes the boss with starting health and position.
    /// </summary>
    protected void Initialize(int maxHealth, Vector2 homePosition, IAudioManager audioManager)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        HomePosition = homePosition;
        Position = homePosition;
        ElapsedTime = 0f;
        AudioManager = audioManager;
    }

    /// <summary>
    /// Base update method that tracks elapsed time and calls boss-specific update logic.
    /// </summary>
    public override void Update(float deltaTime)
    {
        if (!IsActive || IsDefeated)
        {
            return;
        }

        ElapsedTime += deltaTime;

        // Handle flashing effect during damage flash (same as player)
        if (_damageFlashTimer > 0)
        {
            _damageFlashTimer -= deltaTime;

            _flashTimer += deltaTime;
            if (_flashTimer >= FlashInterval)
            {
                _flashTimer = 0f;
                _showRedFlash = !_showRedFlash;
                // Flash between red and default (null = reset to default color)
                RenderColor = _showRedFlash ? Spectre.Console.Color.Red : null;
            }
        }
        else
        {
            // Flash timer ended - reset to normal appearance (no color)
            if (RenderColor.HasValue)
            {
                RenderColor = null;
                _showRedFlash = false;
            }
        }

        // Call boss-specific update logic
        UpdateBehavior(deltaTime);

        base.Update(deltaTime);
    }

    /// <summary>
    /// Override this method to implement boss-specific behavior patterns.
    /// </summary>
    protected abstract void UpdateBehavior(float deltaTime);

    /// <summary>
    /// Applies damage to the boss.
    /// </summary>
    public virtual void TakeDamage(int damage)
    {
        if (IsDefeated)
        {
            return;
        }

        CurrentHealth -= damage;
        CurrentHealth = Math.Max(0, CurrentHealth);

        // Play impact sound effect
        _ = AudioManager?.PlayOneShot(AudioResources.SoundEffects.Impact_002);

        // Trigger red flash effect - immediately show red
        _damageFlashTimer = DamageFlashDuration;
        _flashTimer = 0f;
        _showRedFlash = true;
        RenderColor = Spectre.Console.Color.Red;

        OnDamaged?.Invoke(this, damage);

        if (IsDefeated)
        {
            HandleDefeat();
        }
    }

    /// <summary>
    /// Called when the boss is defeated.
    /// </summary>
    protected virtual void HandleDefeat()
    {
        IsActive = false;
        OnDefeated?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called when this boss collides with another entity.
    /// </summary>
    public override void OnCollision(ICollidable other)
    {
        // Projectiles should damage the boss
        if (other is Projectile projectile && projectile.Layer == CollisionLayer.PlayerProjectile)
        {
            TakeDamage(10); // Default damage, can be customized
            projectile.IsActive = false; // Destroy the projectile
        }
    }
}
