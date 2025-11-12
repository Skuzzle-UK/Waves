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
    public Enemy()
    {
        Speed = GameConstants.Enemy.DefaultSpeed;
        ClampToBounds = GameConstants.Enemy.ClampToBounds;
        RenderPriority = GameConstants.Enemy.RenderPriority;

        // Set collision properties
        Layer = CollisionLayer.Enemy;
        CollidesWith = CollisionLayer.Player | CollisionLayer.PlayerProjectile;
    }

    /// <summary>
    /// Handles collision with other entities.
    /// When hit by player, the enemy disappears.
    /// </summary>
    public override void OnCollision(ICollidable other)
    {
        if (other.Layer == CollisionLayer.Player)
        {
            // Wall disappears when player touches it
            IsActive = false;
        }
        else if (other.Layer == CollisionLayer.PlayerProjectile)
        {
            // Enemy can be destroyed by projectiles too
            IsActive = false;
        }
    }
}
