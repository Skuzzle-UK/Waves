using Waves.Core.Assets.BaseAssets;
using Waves.Core.Collision;
using Waves.Core.Enums;
using Waves.Core.Interfaces;
using Waves.Core.Maths;

namespace Waves.Entities;

/// <summary>
/// Base class for all game entities that can move and update.
/// </summary>
public abstract class BaseEntity : IRenderable, ICollidable, IDisposableEntity
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
    /// When false, the entity is temporarily disabled but can be re-enabled.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether this entity has been permanently disposed.
    /// Once disposed, the entity should be removed from all systems and cannot be reused.
    /// </summary>
    public bool IsDisposed { get; private set; }

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

    /// <summary>
    /// The collision layer this entity belongs to. Default is None (no collisions).
    /// </summary>
    public virtual CollisionLayer Layer { get; set; } = CollisionLayer.None;

    /// <summary>
    /// The collision layers this entity can collide with. Default is None (no collisions).
    /// </summary>
    public virtual CollisionLayer CollidesWith { get; set; } = CollisionLayer.None;

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

    /// <summary>
    /// Computes the bounding box for collision detection based on position and asset dimensions.
    /// </summary>
    public virtual BoundingBox GetBounds()
    {
        if (Asset != null)
        {
            float halfWidth = Asset.Width / 2f;
            float halfHeight = Asset.Height / 2f;

            return new BoundingBox(
                left: Position.X - halfWidth,
                right: Position.X + halfWidth,
                top: Position.Y - halfHeight,
                bottom: Position.Y + halfHeight
            );
        }

        // Single character entity (1x1 bounds)
        return new BoundingBox(
            left: Position.X - 0.5f,
            right: Position.X + 0.5f,
            top: Position.Y - 0.5f,
            bottom: Position.Y + 0.5f
        );
    }

    /// <summary>
    /// Called when this entity collides with another collidable entity.
    /// Override to implement collision response behavior.
    /// </summary>
    public virtual void OnCollision(ICollidable other)
    {
        // Default: no collision response
    }

    /// <summary>
    /// Permanently disposes of this entity, marking it for removal from all systems.
    /// Once disposed, the entity cannot be reused.
    /// Call EntityRegistry.DisposeEntity() instead of calling this directly.
    /// </summary>
    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;
        IsActive = false;
        OnDispose();
    }

    /// <summary>
    /// Called when the entity is being disposed.
    /// Override to implement entity-specific cleanup logic.
    /// </summary>
    protected virtual void OnDispose()
    {
    }
}
