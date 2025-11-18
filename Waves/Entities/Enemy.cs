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

    /// <summary>
    /// Current health of the enemy.
    /// </summary>
    public int Health { get; set; }

    /// <summary>
    /// Event fired when the enemy takes damage.
    /// </summary>
    public event EventHandler<int>? OnDamaged;

    /// <summary>
    /// Event fired when the enemy dies (health reaches 0).
    /// </summary>
    public event EventHandler? OnDeath;

    public Enemy(IEntityRegistry entityRegistry)
    {
        _entityRegistry = entityRegistry ?? throw new ArgumentNullException(nameof(entityRegistry));

        Speed = GameConstants.Enemy.DefaultSpeed;
        ClampToBounds = GameConstants.Enemy.ClampToBounds;
        RenderPriority = GameConstants.Enemy.RenderPriority;
        Health = GameConstants.Enemy.DefaultHealth;

        // Set collision properties
        Layer = CollisionLayer.Enemy;
        CollidesWith = CollisionLayer.Player | CollisionLayer.PlayerProjectile;
    }

    /// <summary>
    /// Reduces the enemy's health by the specified amount.
    /// Fires OnDamaged event and handles death when health reaches 0.
    /// </summary>
    /// <param name="damage">The amount of damage to apply.</param>
    public void TakeDamage(int damage)
    {
        Health -= damage;
        OnDamaged?.Invoke(this, damage);

        if (Health <= 0)
        {
            OnDeath?.Invoke(this, EventArgs.Empty);
            _entityRegistry.DisposeEntity(this);
        }
    }

    /// <summary>
    /// Handles collision with other entities.
    /// Takes damage when hit by projectiles, instantly killed when touched by player.
    /// </summary>
    public override void OnCollision(ICollidable other)
    {
        if (other.Layer == CollisionLayer.Player)
        {
            // Enemy is instantly destroyed when player touches it
            TakeDamage(Health); // Kill instantly
        }
        else if (other.Layer == CollisionLayer.PlayerProjectile)
        {
            // Enemy takes damage from projectiles
            TakeDamage(10); // TODO: Make projectile damage configurable
        }
    }
}
