using Waves.Assets;
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
    private Vector2 _acceleration;
    private Action<int>? _onTakeDamage;
    private float _invulnerabilityTimer = 0f;
    private float _landmassInvulnerabilityTimer = 0f;
    private float _flashTimer = 0f;
    private bool _showRedFlash = false;
    private bool _skipDragNextFrame = false;
    private float _bounceSpeedTimer = 0f;
    private const float FlashInterval = 0.2f; // Flash every 0.2 seconds
    private const float BounceSpeedDuration = 0.2f; // Allow higher speed for 0.2 seconds after bounce

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
        CollidesWith = CollisionLayer.Enemy | CollisionLayer.EnemyProjectile | CollisionLayer.Obstacle;
    }

    /// <summary>
    /// Initialises the player with all necessary dependencies and systems.
    /// This method sets up input, weapons, and registers with required systems.
    /// </summary>
    /// <param name="inputSystem">The input system for player control.</param>
    /// <param name="entityRegistry">The registry for system registration.</param>
    /// <param name="projectileSpawner">The spawner for player projectiles.</param>
    /// <param name="onTakeDamage">Callback to invoke when player takes damage.</param>
    public void Initialise(
        InputSystem inputSystem,
        IEntityRegistry entityRegistry,
        ProjectileSpawner projectileSpawner,
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
        if (_bounceSpeedTimer > 0)
        {
            _bounceSpeedTimer -= deltaTime;
        }

        // Handle flashing effect during invulnerability (if either timer active)
        if (_invulnerabilityTimer > 0 || _landmassInvulnerabilityTimer > 0)
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
            // Both timers ended - reset to normal appearance (no color)
            if (RenderColor.HasValue)
            {
                RenderColor = null;
                _showRedFlash = false;
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
                // Take damage
                _onTakeDamage?.Invoke(GameConstants.Player.LandmassDamage);

                // Apply bounce physics based on wall position
                ApplyBounce(landmass.LanePosition);

                // Start invulnerability window AFTER applying bounce
                // This ensures the timer is set even if multiple landmass chunks collide in the same frame
                _landmassInvulnerabilityTimer = GameConstants.Player.LandmassInvulnerabilityDuration;

            }

            return; // Early return - don't process as generic obstacle
        }

        // Handle enemy/projectile collisions (instant damage, no invulnerability)
        if ((other.Layer & (CollisionLayer.Enemy | CollisionLayer.EnemyProjectile)) != 0)
        {
            _onTakeDamage?.Invoke(10); // TODO: Make damage configurable per entity type
            return;
        }

        // Handle terrain obstacles (islands, boats) with invulnerability cooldown
        if ((other.Layer & CollisionLayer.Obstacle) != 0 && _invulnerabilityTimer <= 0)
        {

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
}
