using Waves.Core.Configuration;
using Waves.Core.Enums;
using Waves.Core.Interfaces;
using Waves.Core.Maths;

namespace Waves.Entities;

/// <summary>
/// Represents a projectile entity that moves through the game world.
/// </summary>
public class Projectile : BaseEntity
{
    // Asset system now implemented - entities can use IAsset for multi-character display
    /// <summary>
    /// Maximum distance this projectile can travel before becoming inactive.
    /// </summary>
    public float MaxDistance { get; set; }

    private readonly Vector2 _startPosition;
    private float _distanceTraveled;

    public Projectile()
    {
        DisplayCharacter = '>';
        ClampToBounds = false;  // Projectiles can go off-screen
        RenderPriority = GameConstants.Projectile.RenderPriority;
        MaxDistance = GameConstants.Projectile.MaxDistance;
        Speed = GameConstants.Projectile.EntitySpeed;
        _startPosition = Position;
        _distanceTraveled = 0f;

        // Set collision properties
        Layer = CollisionLayer.PlayerProjectile;
        CollidesWith = CollisionLayer.Enemy;
    }

    /// <summary>
    /// Updates the projectile, including distance tracking and deactivation when max distance is reached.
    /// </summary>
    public override void Update(float deltaTime)
    {
        if (!IsActive)
        {
            return;
        }

        // Store old position to calculate distance moved
        Vector2 oldPosition = Position;

        // Apply base movement
        base.Update(deltaTime);

        // Track distance traveled
        _distanceTraveled += Vector2.Distance(oldPosition, Position);

        // Deactivate if max distance exceeded
        if (_distanceTraveled >= MaxDistance)
        {
            IsActive = false;
        }
    }

    /// <summary>
    /// Handles collision with other entities. Deactivates the projectile on collision with enemies.
    /// </summary>
    public override void OnCollision(ICollidable other)
    {
        // Projectile is destroyed when it hits anything it collides with (enemies)
        if ((other.Layer & CollidesWith) != 0)
        {
            IsActive = false;
        }
    }
}
