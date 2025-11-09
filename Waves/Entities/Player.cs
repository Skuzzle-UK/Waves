using Waves.Core.Maths;
using Waves.Systems;

namespace Waves.Entities;

/// <summary>
/// Player entity with physics-based movement controlled by WASD input.
/// </summary>
public class Player : BaseEntity
{
    // TODO: Look at a way we can register the projectile spawner to the player as we might want to do similar to enemies
    private readonly InputSystem _inputSystem;
    private Vector2 _acceleration;

    public char Character { get; set; }
    public float Mass { get; set; }
    public float Drag { get; set; }
    public Vector2 MaxSpeed { get; set; }
    public float MoveForce { get; set; }

    public Player(InputSystem inputSystem, Vector2 position, char character = '@')
    {
        _inputSystem = inputSystem;
        Position = position;
        Character = character;
        Mass = 2.0f;
        Drag = 0.9f;
        MaxSpeed = new Vector2(20.0f, 5.0f); // Horizontal faster than vertical
        MoveForce = 300f;
        Speed = 1f; // Not used in physics-based movement
        _acceleration = Vector2.Zero;
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

        // Get movement input from WASD keys
        Vector2 movement = _inputSystem.GetMovementInput(MoveForce);
        if (movement.Length > 0)
        {
            AddForce(movement);
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
