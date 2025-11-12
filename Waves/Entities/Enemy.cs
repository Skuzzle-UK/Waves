using Waves.Core.Configuration;
using Waves.Core.Enums;
using Waves.Core.Interfaces;

namespace Waves.Entities;

/// <summary>
/// Represents an enemy entity in the game.
/// Enemies can be stationary obstacles or moving threats.
/// </summary>
public class Enemy : BaseEntity
{
    private readonly IEntityRegistry _entityRegistry;

    public Enemy(IEntityRegistry entityRegistry)
    {
        _entityRegistry = entityRegistry ?? throw new ArgumentNullException(nameof(entityRegistry));

        Speed = GameConstants.Enemy.DefaultSpeed;
        ClampToBounds = GameConstants.Enemy.ClampToBounds;
        RenderPriority = GameConstants.Enemy.RenderPriority;

        // Set collision properties
        Layer = CollisionLayer.Enemy;
        CollidesWith = CollisionLayer.Player | CollisionLayer.PlayerProjectile;
    }

    /// <summary>
    /// Handles collision with other entities.
    /// Disposes the enemy when hit by player or projectiles.
    /// </summary>
    public override void OnCollision(ICollidable other)
    {
        if (other.Layer == CollisionLayer.Player)
        {
            // Enemy is destroyed when player touches it
            _entityRegistry.DisposeEntity(this);
        }
        else if (other.Layer == CollisionLayer.PlayerProjectile)
        {
            // Enemy is destroyed by projectiles
            _entityRegistry.DisposeEntity(this);
        }
    }
}
