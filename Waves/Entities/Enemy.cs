using Waves.Core.AI;
using Waves.Core.Configuration;
using Waves.Core.Enums;
using Waves.Core.Interfaces;

namespace Waves.Entities;

/// <summary>
/// Represents an enemy entity in the game.
/// Enemies can be stationary obstacles or moving threats.
/// </summary>
public class Enemy : BaseEntity
{
    private readonly IEntityRegistry _entityRegistry;

    /// <summary>
    /// Current health of the enemy.
    /// </summary>
    public int Health { get; set; }

    /// <summary>
    /// Event fired when the enemy takes damage.
    /// </summary>
    public event EventHandler<int>? OnDamaged;

    /// <summary>
    /// Event fired when the enemy dies (health reaches 0).
    /// </summary>
    public event EventHandler? OnDeath;

    /// <summary>
    /// AI behavior controlling this enemy's movement and decisions.
    /// </summary>
    public IAIBehavior? AIBehavior { get; set; }

    /// <summary>
    /// Shooting pattern for this enemy's projectiles.
    /// </summary>
    public IShootingPattern? ShootingPattern { get; set; }

    /// <summary>
    /// Whether this enemy applies knockback on collision.
    /// </summary>
    public bool ApplyKnockback { get; set; }

    // Entrance animation state
    private const float EntranceDuration = 1.5f; // 1.5 seconds entrance
    private float _entranceTimer;
    private Core.Maths.Vector2 _spawnPosition;
    private Core.Maths.Vector2 _finalPosition;
    private bool _entranceComplete;

    // Damage flash state
    private float _damageFlashTimer = 0f;
    private float _flashTimer = 0f;
    private bool _showWhiteFlash = false;
    private const float DamageFlashDuration = 0.2f;
    private const float FlashInterval = 0.2f;

    private float _shootCooldown;

    public Enemy(IEntityRegistry entityRegistry)
    {
        _entityRegistry = entityRegistry ?? throw new ArgumentNullException(nameof(entityRegistry));

        Speed = GameConstants.Enemy.DefaultSpeed;
        ClampToBounds = GameConstants.Enemy.ClampToBounds;
        RenderPriority = GameConstants.Enemy.RenderPriority;
        Health = GameConstants.Enemy.DefaultHealth;

        // Set collision properties
        Layer = CollisionLayer.Enemy;
        CollidesWith = CollisionLayer.Player | CollisionLayer.PlayerProjectile;

        // Set enemies to be red by default
        RenderColor = Spectre.Console.Color.Red;

        // Initialize entrance animation as complete by default
        _entranceComplete = true;
    }

    /// <summary>
    /// Sets up the entrance animation for this enemy.
    /// Call this to make the enemy slide in from the spawn position to the final position.
    /// </summary>
    public void SetupEntranceAnimation(Core.Maths.Vector2 spawnPosition, Core.Maths.Vector2 finalPosition)
    {
        _spawnPosition = spawnPosition;
        _finalPosition = finalPosition;
        _entranceTimer = 0f;
        _entranceComplete = false;
        Position = spawnPosition;
    }

    /// <summary>
    /// Reduces the enemy's health by the specified amount.
    /// Fires OnDamaged event and handles death when health reaches 0.
    /// </summary>
    /// <param name="damage">The amount of damage to apply.</param>
    public void TakeDamage(int damage)
    {
        Health -= damage;
        OnDamaged?.Invoke(this, damage);

        // Trigger white flash effect - immediately show white (null color)
        _damageFlashTimer = DamageFlashDuration;
        _flashTimer = 0f;
        _showWhiteFlash = true;
        RenderColor = null; // Flash to white/default

        if (Health <= 0)
        {
            OnDeath?.Invoke(this, EventArgs.Empty);
            _entityRegistry.DisposeEntity(this);
        }
    }

    /// <summary>
    /// Updates the shooting cooldown timer.
    /// </summary>
    /// <param name="deltaTime">Time elapsed since last update.</param>
    public void UpdateShootCooldown(float deltaTime)
    {
        if (_shootCooldown > 0)
        {
            _shootCooldown -= deltaTime;
        }
    }

    /// <summary>
    /// Checks if the enemy can shoot (cooldown has expired).
    /// </summary>
    /// <returns>True if the enemy can shoot, false otherwise.</returns>
    public bool CanShoot() => _shootCooldown <= 0 && ShootingPattern != null;

    /// <summary>
    /// Resets the shooting cooldown based on the current shooting pattern.
    /// </summary>
    public void ResetShootCooldown()
    {
        if (ShootingPattern != null)
        {
            _shootCooldown = ShootingPattern.GetCooldownTime();
        }
    }

    /// <summary>
    /// Updates the enemy state and checks for off-screen deactivation.
    /// </summary>
    public override void Update(float deltaTime)
    {
        // Update cooldown timer
        UpdateShootCooldown(deltaTime);

        // Handle flashing effect during damage flash (reverse of boss - flash white instead of red)
        if (_damageFlashTimer > 0)
        {
            _damageFlashTimer -= deltaTime;

            _flashTimer += deltaTime;
            if (_flashTimer >= FlashInterval)
            {
                _flashTimer = 0f;
                _showWhiteFlash = !_showWhiteFlash;
                // Flash between white (null) and red
                RenderColor = _showWhiteFlash ? null : Spectre.Console.Color.Red;
            }
        }
        else
        {
            // Flash timer ended - reset to red (normal appearance)
            if (!RenderColor.HasValue || RenderColor.Value != Spectre.Console.Color.Red)
            {
                RenderColor = Spectre.Console.Color.Red;
                _showWhiteFlash = false;
            }
        }

        // Handle entrance animation phase
        if (!_entranceComplete)
        {
            _entranceTimer += deltaTime;

            // Calculate progress (0.0 to 1.0)
            float progress = Math.Min(_entranceTimer / EntranceDuration, 1.0f);

            // Linear interpolation from spawn to final position
            float currentX = _spawnPosition.X + (_finalPosition.X - _spawnPosition.X) * progress;
            float currentY = _spawnPosition.Y + (_finalPosition.Y - _spawnPosition.Y) * progress;
            Position = new Core.Maths.Vector2(currentX, currentY);

            // Check if entrance is complete
            if (_entranceTimer >= EntranceDuration)
            {
                _entranceComplete = true;
                Position = _finalPosition;
            }

            // Skip normal AI behavior during entrance
            return;
        }

        // Call base update for movement
        base.Update(deltaTime);

        // Auto-deactivate if completely off-screen and not clamped to bounds
        if (!ClampToBounds && IsActive)
        {
            float gameWidth = AppWrapper.GameAreaWidth;
            float gameHeight = AppWrapper.GameAreaHeight - GameConstants.Display.GameGridHeightOffset;

            // Check if enemy is completely off-screen (with margin for asset size)
            float margin = Asset?.Width ?? 10f;
            bool offScreenLeft = Position.X < -margin;
            bool offScreenRight = Position.X > gameWidth + margin;
            bool offScreenTop = Position.Y < -margin;
            bool offScreenBottom = Position.Y > gameHeight + margin;

            if (offScreenLeft || offScreenRight || offScreenTop || offScreenBottom)
            {
                IsActive = false;
            }
        }
    }

    /// <summary>
    /// Handles collision with other entities.
    /// Takes damage when hit by projectiles, instantly killed when touched by player.
    /// </summary>
    public override void OnCollision(ICollidable other)
    {
        if (other.Layer == CollisionLayer.Player)
        {
            // Enemy is instantly destroyed when player touches it
            TakeDamage(Health); // Kill instantly
        }
        else if (other.Layer == CollisionLayer.PlayerProjectile)
        {
            // Enemy takes damage from projectiles
            TakeDamage(10); // TODO: Make projectile damage configurable
        }
    }
}
