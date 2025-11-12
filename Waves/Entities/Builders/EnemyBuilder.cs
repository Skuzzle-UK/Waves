using Waves.Core.Assets.BaseAssets;
using Waves.Core.Maths;

namespace Waves.Entities.Builders;

/// <summary>
/// Builder for creating Enemy entities with a fluent API.
/// Provides a flexible way to configure enemies before instantiation.
/// </summary>
public class EnemyBuilder
{
    private Vector2 _position = Vector2.Zero;
    private Vector2 _velocity = Vector2.Zero;
    private float _speed = 0f; // Default: stationary
    private IAsset? _asset = null;

    // Private constructor to enforce use of Create() factory method
    private EnemyBuilder() { }

    /// <summary>
    /// Creates a new EnemyBuilder instance.
    /// </summary>
    public static EnemyBuilder Create() => new();

    /// <summary>
    /// Sets the initial position of the enemy.
    /// </summary>
    public EnemyBuilder WithPosition(Vector2 position)
    {
        _position = position;
        return this;
    }

    /// <summary>
    /// Sets the velocity direction of the enemy.
    /// The vector will be normalized automatically.
    /// </summary>
    public EnemyBuilder WithVelocity(Vector2 velocity)
    {
        _velocity = velocity;
        return this;
    }

    /// <summary>
    /// Sets the movement speed of the enemy.
    /// Default is 0 (stationary).
    /// </summary>
    public EnemyBuilder WithSpeed(float speed)
    {
        _speed = speed;
        return this;
    }

    /// <summary>
    /// Sets the visual asset for the enemy.
    /// </summary>
    public EnemyBuilder WithAsset(IAsset asset)
    {
        _asset = asset;
        return this;
    }

    /// <summary>
    /// Builds and returns a configured Enemy instance.
    /// </summary>
    public Enemy Build()
    {
        Enemy enemy = new Enemy
        {
            Position = _position,
            Velocity = _velocity.Length > 0 ? _velocity.Normalized() : Vector2.Zero,
            Speed = _speed,
            Asset = _asset
        };

        return enemy;
    }
}
