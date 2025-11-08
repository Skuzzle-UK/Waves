using Waves.Core.Maths;

namespace Waves.Entities;

/// <summary>
/// Base class for all game entities that can move and update.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier for this entity.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Current position of the entity in 2D space.
    /// </summary>
    public Vector2 Position { get; set; }

    /// <summary>
    /// Velocity vector (direction and magnitude of movement before speed is applied).
    /// </summary>
    public Vector2 Velocity { get; set; }

    /// <summary>
    /// Speed multiplier applied to velocity. Default is 100 units per second.
    /// </summary>
    public float Speed { get; set; }

    /// <summary>
    /// Whether this entity is active and should be updated.
    /// </summary>
    public bool IsActive { get; set; }

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        Position = Vector2.Zero;
        Velocity = Vector2.Zero;
        Speed = 100f;
        IsActive = true;
    }

    /// <summary>
    /// Updates the entity's state. Default implementation applies velocity-based movement.
    /// Override to add custom behavior.
    /// </summary>
    /// <param name="deltaTime">Time elapsed since last update in seconds (fixed at 0.016 for 60 FPS).</param>
    public virtual void Update(float deltaTime)
    {
        if (!IsActive)
        {
            return;
        }

        // Apply velocity-based movement: Position += Velocity * Speed * DeltaTime
        Position += Velocity * Speed * deltaTime;
    }
}
