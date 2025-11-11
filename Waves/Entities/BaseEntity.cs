using Waves.Core.Assets;
using Waves.Core.Interfaces;
using Waves.Core.Maths;

namespace Waves.Entities;

/// <summary>
/// Base class for all game entities that can move and update.
/// </summary>
public abstract class BaseEntity : IRenderable
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

    /// <summary>
    /// The character used to display this entity.
    /// </summary>
    public virtual char DisplayCharacter { get; set; } = '?';

    /// <summary>
    /// The visual asset used to display this entity.
    /// When set, this takes precedence over DisplayCharacter.
    /// </summary>
    public virtual IAsset? Asset { get; set; }

    /// <summary>
    /// Whether this entity should be clamped to screen bounds.
    /// </summary>
    public virtual bool ClampToBounds { get; set; } = false;

    /// <summary>
    /// Rendering priority for layering (higher values render on top).
    /// </summary>
    public virtual int RenderPriority { get; set; } = 0;

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

        // Update the asset if it exists (for animations)
        Asset?.Update(deltaTime);

        // Apply velocity-based movement: Position += Velocity * Speed * DeltaTime
        Position += Velocity * Speed * deltaTime;
    }
}
