using Waves.Assets.BaseAssets;
using Waves.Core.Maths;

namespace Waves.Entities.Builders;

/// <summary>
/// Builder for creating enemy projectile entities with a fluent API.
/// </summary>
public class EnemyProjectileBuilder
{
    private Vector2 _position = Vector2.Zero;
    private Vector2 _direction = Vector2.Left;
    private float _speed = 120f;
    private int _damage = 10;
    private IAsset? _asset = null;

    private EnemyProjectileBuilder()
    {
    }

    /// <summary>
    /// Creates a new enemy projectile builder.
    /// </summary>
    public static EnemyProjectileBuilder Create() => new();

    /// <summary>
    /// Sets the position of the projectile.
    /// </summary>
    public EnemyProjectileBuilder WithPosition(Vector2 position)
    {
        _position = position;
        return this;
    }

    /// <summary>
    /// Sets the direction of the projectile.
    /// </summary>
    public EnemyProjectileBuilder WithDirection(Vector2 direction)
    {
        _direction = direction;
        return this;
    }

    /// <summary>
    /// Sets the speed of the projectile.
    /// </summary>
    public EnemyProjectileBuilder WithSpeed(float speed)
    {
        _speed = speed;
        return this;
    }

    /// <summary>
    /// Sets the damage of the projectile.
    /// </summary>
    public EnemyProjectileBuilder WithDamage(int damage)
    {
        _damage = damage;
        return this;
    }

    /// <summary>
    /// Sets the asset for the projectile.
    /// </summary>
    public EnemyProjectileBuilder WithAsset(IAsset asset)
    {
        _asset = asset;
        return this;
    }

    /// <summary>
    /// Builds the enemy projectile with the configured properties.
    /// </summary>
    public EnemyProjectile Build()
    {
        return new EnemyProjectile
        {
            Position = _position,
            Velocity = _direction.Normalized(),
            Speed = _speed,
            Damage = _damage,
            Asset = _asset
        };
    }
}
