using Waves.Core.Configuration;
using Waves.Core.Enums;
using Waves.Core.Interfaces;

namespace Waves.Entities;

/// <summary>
/// Represents a collectable item that grants bonuses to the player.
/// </summary>
public class Collectable : BaseEntity
{
    /// <summary>
    /// The type of collectable and its effect.
    /// </summary>
    public CollectableType Type { get; set; }

    /// <summary>
    /// Maximum distance this collectable can travel before becoming inactive.
    /// </summary>
    public float MaxDistance { get; set; }

    private readonly Core.Maths.Vector2 _startPosition;
    private float _distanceTraveled;

    public Collectable()
    {
        ClampToBounds = false;
        RenderPriority = 7; // Between projectiles and enemies
        Speed = 30f; // Slow scroll speed
        MaxDistance = 1000f;
        _startPosition = Position;
        _distanceTraveled = 0f;

        Layer = CollisionLayer.Collectable;
        CollidesWith = CollisionLayer.Player;
    }

    /// <summary>
    /// Updates the collectable, including distance tracking and deactivation when max distance is reached.
    /// </summary>
    public override void Update(float deltaTime)
    {
        if (!IsActive)
        {
            return;
        }

        Core.Maths.Vector2 oldPosition = Position;

        base.Update(deltaTime);

        _distanceTraveled += Core.Maths.Vector2.Distance(oldPosition, Position);

        if (_distanceTraveled >= MaxDistance)
        {
            IsActive = false;
        }
    }

    /// <summary>
    /// Handles collision with other entities. Deactivates the collectable on collision with the player.
    /// </summary>
    public override void OnCollision(ICollidable other)
    {
        if (other.Layer == CollisionLayer.Player)
        {
            IsActive = false;
        }
    }
}
