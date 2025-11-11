using Waves.Core.Assets;
using Waves.Core.Configuration;
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

    public float Mass { get; set; }
    public float Drag { get; set; }
    public Vector2 MaxSpeed { get; set; }
    public float MoveForce { get; set; }

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
    }

    /// <summary>
    /// Initializes the player with all necessary dependencies and systems.
    /// This method sets up input, weapons, and registers with required systems.
    /// </summary>
    /// <param name="inputSystem">The input system for player control.</param>
    /// <param name="entityRegistry">The registry for system registration.</param>
    /// <param name="projectileSpawner">The spawner for player projectiles.</param>
    public void Initialize(
        InputSystem inputSystem,
        IEntityRegistry entityRegistry,
        ProjectileSpawner projectileSpawner)
    {
        // Create and attach input provider
        var inputProvider = new PlayerInputProvider(inputSystem, MoveForce);
        SetInputProvider(inputProvider);

        // Register with entity systems
        entityRegistry.RegisterEntity(this);

        // Set up weapons/projectiles
        projectileSpawner.SetPlayer(this);
        projectileSpawner.SetInputProvider(inputProvider);
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

        // Apply drag
        Velocity *= Drag;

        // Clamp velocity to max speed (for each axis independently)
        float clampedX = Math.Clamp(Velocity.X, -MaxSpeed.X, MaxSpeed.X);
        float clampedY = Math.Clamp(Velocity.Y, -MaxSpeed.Y, MaxSpeed.Y);
        Velocity = new Vector2(clampedX, clampedY);

        // Apply velocity to position
        Position += Velocity * deltaTime;

        // Reset acceleration for next frame
        _acceleration = Vector2.Zero;
    }
}
