using Waves.Core.Collision;
using Waves.Core.Enums;
using Waves.Core.Maths;

namespace Waves.Core.Interfaces;

public interface ICollidable
{
    /// <summary>
    /// The position of the entity
    /// </summary>
    Vector2 Position { get; }

    /// <summary>
    /// The collision layer this entity belongs to
    /// </summary>
    CollisionLayer Layer { get; }

    /// <summary>
    /// The collision layers this entity can collide with (bitwise flags)
    /// </summary>
    CollisionLayer CollidesWith { get; }

    // TODO: Add a parameter to allow modification of the bounding box, e.g. shrink it as the whole player bounds is quite large.
    /// <summary>
    /// Computes the bounding box for collision detection (computed on-the-fly)
    /// </summary>
    BoundingBox GetBounds();

    /// <summary>
    /// Called when this entity collides with another collidable entity
    /// </summary>
    void OnCollision(ICollidable other);
}
