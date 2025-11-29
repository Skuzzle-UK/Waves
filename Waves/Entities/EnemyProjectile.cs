using Waves.Core.Configuration;
using Waves.Core.Enums;
using Waves.Core.Interfaces;
using Waves.Core.Maths;

namespace Waves.Entities;

/// <summary>
/// Represents a projectile fired by an enemy entity.
/// </summary>
public class EnemyProjectile : BaseEntity
{
    /// <summary>
    /// Damage dealt to the player when this projectile hits.
    /// </summary>
    public int Damage { get; set; }

    /// <summary>
    /// Maximum distance this projectile can travel before becoming inactive.
    /// </summary>
    public float MaxDistance { get; set; }

    private readonly Vector2 _startPosition;
    private float _distanceTraveled;

    public EnemyProjectile()
    {
        DisplayCharacter = '<';
        ClampToBounds = false;
        RenderPriority = GameConstants.EnemyProjectile.RenderPriority;
        MaxDistance = GameConstants.EnemyProjectile.MaxDistance;
        Speed = GameConstants.EnemyProjectile.DefaultSpeed;
        Damage = GameConstants.EnemyProjectile.DefaultDamage;
        _startPosition = Position;
        _distanceTraveled = 0f;

        Layer = CollisionLayer.EnemyProjectile;
        CollidesWith = CollisionLayer.Player;
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

        Vector2 oldPosition = Position;

        base.Update(deltaTime);

        _distanceTraveled += Vector2.Distance(oldPosition, Position);

        if (_distanceTraveled >= MaxDistance)
        {
            IsActive = false;
        }
    }

    /// <summary>
    /// Handles collision with other entities. Deactivates the projectile on collision with the player.
    /// </summary>
    public override void OnCollision(ICollidable other)
    {
        if (other.Layer == CollisionLayer.Player)
        {
            IsActive = false;
        }
    }
}
