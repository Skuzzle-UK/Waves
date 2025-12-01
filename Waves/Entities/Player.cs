using Waves.Assets;
using Waves.Assets.Audio;
using Waves.Core;
using Waves.Core.Configuration;
using Waves.Core.Enums;
using Waves.Core.Interfaces;
using Waves.Core.Maths;
using Waves.Systems;

namespace Waves.Entities;

/// <summary>
/// Player entity with physics-based movement controlled by an input provider.
/// </summary>
public class Player : BaseEntity
{
    // TODO: Look at a way we can register the projectile spawner to the player as we might want to do similar to enemies
    private IInputProvider? _inputProvider;
    private IAudioManager? _audioManager;
    private Vector2 _acceleration;
    private Action<int>? _onTakeDamage;
    private float _invulnerabilityTimer = 0f;
    private float _landmassInvulnerabilityTimer = 0f;
    private float _powerUpInvulnerabilityTimer = 0f; // 10-second invulnerability from power-up
    private float _flashTimer = 0f;
    private bool _showRedFlash = false;
    private bool _showBlueFlash = false;
    private bool _skipDragNextFrame = false;
    private float _bounceSpeedTimer = 0f;
    private const float FlashInterval = 0.2f; // Flash every 0.2 seconds
    private const float BounceSpeedDuration = 0.2f; // Allow higher speed for 0.2 seconds after bounce
    private const float PowerUpInvulnerabilityDuration = 10.0f; // 10 seconds of invulnerability

    public float Mass { get; set; }
    public float Drag { get; set; }
    public Vector2 MaxSpeed { get; set; }
    public float MoveForce { get; set; }
    public int Health { get; set; } = GameConstants.Player.InitialHealth;

    public Player(Vector2 position)
    {
        Position = position;
        Asset = PlayerAssets.Surfer;
        ClampToBounds = true;  // Players should stay on screen
        RenderPriority = GameConstants.Player.RenderPriority;
        Mass = GameConstants.Player.Mass;
        Drag = GameConstants.Player.DragCoefficient;
        MaxSpeed = new Vector2(GameConstants.Player.MaxSpeedHorizontal, GameConstants.Player.MaxSpeedVertical);
        MoveForce = GameConstants.Player.MoveForce;
        Speed = GameConstants.Player.BaseSpeed; // Not used in physics-based movement
        _acceleration = Vector2.Zero;

        // Set collision properties
        Layer = CollisionLayer.Player;
        CollidesWith = CollisionLayer.Enemy | CollisionLayer.EnemyProjectile | CollisionLayer.Obstacle | CollisionLayer.Collectable;
    }

    /// <summary>
    /// Initialises the player with all necessary dependencies and systems.
    /// This method sets up input, weapons, and registers with required systems.
    /// </summary>
    /// <param name="inputSystem">The input system for player control.</param>
    /// <param name="entityRegistry">The registry for system registration.</param>
    /// <param name="projectileSpawner">The spawner for player projectiles.</param>
    /// <param name="audioManager">The audio manager for playing sound effects.</param>
    /// <param name="onTakeDamage">Callback to invoke when player takes damage.</param>
    public void Initialise(
        InputSystem inputSystem,
        IEntityRegistry entityRegistry,
        ProjectileSpawner projectileSpawner,
        IAudioManager audioManager,
        Action<int> onTakeDamage)
    {
        // Create and attach input provider
        PlayerInputProvider inputProvider = new PlayerInputProvider(inputSystem, MoveForce);
        SetInputProvider(inputProvider);

        // Register with entity systems
        entityRegistry.RegisterEntity(this);

        // Set up weapons/projectiles
        projectileSpawner.SetPlayer(this);
        projectileSpawner.SetInputProvider(inputProvider);

        // Store audio manager reference
        _audioManager = audioManager;

        // Set damage callback
        _onTakeDamage = onTakeDamage;
    }

    /// <summary>
    /// Sets the input provider for this player.
    /// </summary>
    /// <param name="inputProvider">The input provider to use for controlling this player.</param>
    private void SetInputProvider(IInputProvider inputProvider)
    {
        _inputProvider = inputProvider;
    }

    public void AddForce(Vector2 force)
    {
        // F = ma, so a = F/m
        _acceleration += force / Mass;
    }

    public override void Update(float deltaTime)
    {
        if (!IsActive)
        {
            return;
        }

        // Decrement invulnerability timers
        if (_invulnerabilityTimer > 0)
        {
            _invulnerabilityTimer -= deltaTime;
        }
        if (_landmassInvulnerabilityTimer > 0)
        {
            _landmassInvulnerabilityTimer -= deltaTime;
        }
        if (_powerUpInvulnerabilityTimer > 0)
        {
            _powerUpInvulnerabilityTimer -= deltaTime;
        }
        if (_bounceSpeedTimer > 0)
        {
            _bounceSpeedTimer -= deltaTime;
        }

        // Handle flashing effect during invulnerability
        // Power-up invulnerability (blue flash) takes priority over damage invulnerability (red flash)
        if (_powerUpInvulnerabilityTimer > 0)
        {
            _flashTimer += deltaTime;
            if (_flashTimer >= FlashInterval)
            {
                _flashTimer = 0f;
                _showBlueFlash = !_showBlueFlash;
                // Flash between blue and default (null = reset to default color)
                RenderColor = _showBlueFlash ? Spectre.Console.Color.Blue : null;
            }
        }
        else if (_invulnerabilityTimer > 0 || _landmassInvulnerabilityTimer > 0)
        {
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
            // All timers ended - reset to normal appearance (no color)
            if (RenderColor.HasValue)
            {
                RenderColor = null;
                _showRedFlash = false;
                _showBlueFlash = false;
            }
        }

        // Get movement input from the input provider
        if (_inputProvider != null)
        {
            Vector2 movement = _inputProvider.GetMovementVector();
            if (movement.Length > 0)
            {
                AddForce(movement);
            }
        }

        // Apply acceleration to velocity
        Velocity += _acceleration * deltaTime;

        // Apply drag (skip if we just bounced to preserve bounce velocity)
        if (!_skipDragNextFrame)
        {
            Velocity *= Drag;
        }
        else
        {
            _skipDragNextFrame = false;
        }

        // Clamp velocity to max speed (for each axis independently)
        // Allow higher vertical speed temporarily after bounce
        float maxVerticalSpeed = _bounceSpeedTimer > 0 ? MaxSpeed.Y * 10f : MaxSpeed.Y;
        float clampedX = Math.Clamp(Velocity.X, -MaxSpeed.X, MaxSpeed.X);
        float clampedY = Math.Clamp(Velocity.Y, -maxVerticalSpeed, maxVerticalSpeed);
        Velocity = new Vector2(clampedX, clampedY);

        // Apply velocity to position
        Position += Velocity * deltaTime;

        // Reset acceleration for next frame
        _acceleration = Vector2.Zero;
    }

    /// <summary>
    /// Handles collision with other entities.
    /// </summary>
    public override void OnCollision(ICollidable other)
    {
        // Handle landmass collisions with bounce physics
        if (other is Landmass landmass)
        {
            // Only bounce and take damage if not currently invulnerable to landmass
            if (_landmassInvulnerabilityTimer <= 0f)
            {
                // Play impact sound and take damage
                if (_audioManager is not null)
                {
                    _ = _audioManager.PlayOneShot(AudioResources.SoundEffects.Impact_003);
                }
                _onTakeDamage?.Invoke(GameConstants.Player.LandmassDamage);

                // Apply bounce physics based on wall position
                ApplyBounce(landmass.LanePosition);

                // Start invulnerability window AFTER applying bounce
                // This ensures the timer is set even if multiple landmass chunks collide in the same frame
                _landmassInvulnerabilityTimer = GameConstants.Player.LandmassInvulnerabilityDuration;

            }

            return; // Early return - don't process as generic obstacle
        }

        // Handle collectable pickups
        if (other.Layer == CollisionLayer.Collectable && other is Collectable collectable)
        {
            if (collectable.Type == CollectableType.HealthUp)
            {
                ActivateHealthPickup();
            }
            else if (collectable.Type == CollectableType.Invulnerability)
            {
                ActivateInvulnerabilityPickup();
            }
            return;
        }

        // Handle enemy projectile collisions (instant damage, no invulnerability)
        // Skip damage if power-up invulnerability is active
        if (other.Layer == CollisionLayer.EnemyProjectile)
        {
            if (_powerUpInvulnerabilityTimer <= 0)
            {
                if (_audioManager is not null)
                {
                    _ = _audioManager.PlayOneShot(AudioResources.SoundEffects.Impact_003);
                }

                // Get damage from EnemyProjectile or use default for boss projectiles
                int damage = other is EnemyProjectile enemyProj ? enemyProj.Damage : 10;
                _onTakeDamage?.Invoke(damage);
                _invulnerabilityTimer = GameConstants.Player.InvulnerabilityDuration;
            }
            return;
        }

        // Handle enemy collisions (different handling for kamikaze vs normal)
        // Skip damage if power-up invulnerability is active
        if (other.Layer == CollisionLayer.Enemy && other is Enemy enemy)
        {
            if (_invulnerabilityTimer <= 0 && _powerUpInvulnerabilityTimer <= 0)
            {
                if (enemy.ApplyKnockback)
                {
                    // Kamikaze enemies deal more damage and apply knockback
                    _audioManager?.PlayOneShot(AudioResources.SoundEffects.EightBit.Explosion_001);
                    _onTakeDamage?.Invoke(GameConstants.Player.KamikazeDamage);
                    ApplyKamikazeKnockback(enemy.Position);
                }
                else
                {
                    // Normal enemies deal standard damage
                    _onTakeDamage?.Invoke(GameConstants.Player.EnemyDamage);
                }
                _invulnerabilityTimer = GameConstants.Player.InvulnerabilityDuration;
            }
            return;
        }

        // Handle terrain obstacles (islands, boats) with invulnerability cooldown
        // Skip damage if power-up invulnerability is active
        if ((other.Layer & CollisionLayer.Obstacle) != 0 && _invulnerabilityTimer <= 0 && _powerUpInvulnerabilityTimer <= 0)
        {
            _audioManager?.PlayOneShot(AudioResources.SoundEffects.Impact_003);
            _onTakeDamage?.Invoke(GameConstants.Player.TerrainDamage);
            _invulnerabilityTimer = GameConstants.Player.InvulnerabilityDuration;
        }
    }

    /// <summary>
    /// Applies bounce physics when colliding with landmass boundaries.
    /// Reverses and amplifies vertical velocity while preserving horizontal momentum.
    /// </summary>
    /// <param name="wallPosition">The position of the wall (Top or Bottom)</param>
    private void ApplyBounce(LandmassPosition wallPosition)
    {
        // Reverse and amplify vertical velocity for bounce effect
        // Preserve horizontal velocity for smooth surfing flow
        float bounceVelocityY = -Velocity.Y * GameConstants.Player.BounceMultiplier;

        // Don't clamp bounce velocity here - allow it to exceed normal max speed temporarily
        // The velocity clamping in Update() will handle it with the temporary 2x speed limit

        // Apply bounce with directional validation
        // Top wall should always bounce downward (positive Y)
        // Bottom wall should always bounce upward (negative Y)
        if (wallPosition == LandmassPosition.Top)
        {
            // Ensure we're bouncing downward from top wall
            Velocity = new Vector2(Velocity.X, Math.Max(bounceVelocityY, 0));
        }
        else // LandmassPosition.Bottom
        {
            // Ensure we're bouncing upward from bottom wall
            Velocity = new Vector2(Velocity.X, Math.Min(bounceVelocityY, 0));
        }

        // Reset acceleration to prevent lingering input forces from fighting the bounce
        _acceleration = Vector2.Zero;

        // Skip drag on next frame to preserve bounce velocity
        _skipDragNextFrame = true;

        // Allow higher speed temporarily to let bounce velocity through
        _bounceSpeedTimer = BounceSpeedDuration;
    }

    /// <summary>
    /// Applies knockback physics when hit by a kamikaze enemy.
    /// Calculates knockback direction away from the enemy and applies amplified velocity.
    /// </summary>
    /// <param name="enemyPosition">The position of the kamikaze enemy</param>
    private void ApplyKamikazeKnockback(Vector2 enemyPosition)
    {
        // Calculate direction away from enemy
        Vector2 knockbackDirection = Position - enemyPosition;

        // If positions are identical (rare edge case), use random direction
        if (knockbackDirection.Length < 0.1f)
        {
            Random random = new Random();
            float angle = (float)(random.NextDouble() * Math.PI * 2);
            knockbackDirection = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
        else
        {
            knockbackDirection = knockbackDirection.Normalized();
        }

        // Apply knockback velocity (scaled by multiplier)
        Velocity = knockbackDirection * GameConstants.Player.KamikazeKnockbackMultiplier;

        // Reset acceleration to prevent input from fighting the knockback
        _acceleration = Vector2.Zero;

        // Skip drag on next frame to preserve knockback velocity
        _skipDragNextFrame = true;

        // Allow higher speed temporarily to let knockback velocity through
        _bounceSpeedTimer = BounceSpeedDuration;
    }

    /// <summary>
    /// Activates the health pickup, restoring 20 health up to MaxHealth.
    /// </summary>
    private void ActivateHealthPickup()
    {
        int healAmount = 20;
        int newHealth = Math.Min(Health + healAmount, GameConstants.Player.MaxHealth);
        int actualHealed = newHealth - Health;

        if (actualHealed > 0)
        {
            Health = newHealth;
            // Play positive sound effect
            _audioManager?.PlayOneShot(AudioResources.SoundEffects.Impact_002); // TODO: Add proper pickup sound
        }
    }

    /// <summary>
    /// Activates the invulnerability pickup, granting 10 seconds of invulnerability with blue flashing.
    /// </summary>
    private void ActivateInvulnerabilityPickup()
    {
        _powerUpInvulnerabilityTimer = PowerUpInvulnerabilityDuration;
        _flashTimer = 0f;
        _showBlueFlash = true;

        // Play positive sound effect
        _audioManager?.PlayOneShot(AudioResources.SoundEffects.Impact_002); // TODO: Add proper pickup sound
    }
}
