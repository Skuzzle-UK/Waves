using Waves.Assets.BaseAssets;
using Waves.Core.Maths;

namespace Waves.Entities.Builders;

/// <summary>
/// Builder for creating Projectile entities with a fluent API.
/// </summary>
/// <example>
/// var projectile = ProjectileBuilder.Create()
///     .WithSpeed(150)
///     .WithPosition(new Vector2(10, 20))
///     .WithDirection(Vector2.Up)
///     .Build();
/// </example>
public class ProjectileBuilder
{
    private float _speed = 150f;
    private Vector2 _position = Vector2.Zero;
    private Vector2 _velocity = Vector2.Zero;
    private char _displayChar = '*';
    private IAsset? _asset = null;
    private float _maxDistance = 1000f;

    private ProjectileBuilder() { }

    /// <summary>
    /// Creates a new ProjectileBuilder instance.
    /// </summary>
    public static ProjectileBuilder Create() => new();

    /// <summary>
    /// Sets the speed of the projectile in units per second.
    /// </summary>
    public ProjectileBuilder WithSpeed(float speed)
    {
        _speed = speed;
        return this;
    }

    /// <summary>
    /// Sets the starting position of the projectile.
    /// </summary>
    public ProjectileBuilder WithPosition(Vector2 position)
    {
        _position = position;
        return this;
    }

    /// <summary>
    /// Sets the starting position using X and Y coordinates.
    /// </summary>
    public ProjectileBuilder WithPosition(float x, float y)
    {
        _position = new Vector2(x, y);
        return this;
    }

    /// <summary>
    /// Sets the velocity vector directly (will be normalized automatically).
    /// </summary>
    public ProjectileBuilder WithVelocity(Vector2 velocity)
    {
        _velocity = velocity;
        return this;
    }

    /// <summary>
    /// Sets the direction the projectile moves (automatically normalized).
    /// </summary>
    public ProjectileBuilder WithDirection(Vector2 direction)
    {
        _velocity = direction.Normalized();
        return this;
    }

    /// <summary>
    /// Sets the display character used to render the projectile.
    /// </summary>
    public ProjectileBuilder WithDisplayChar(char displayChar)
    {
        _displayChar = displayChar;
        return this;
    }

    /// <summary>
    /// Sets the maximum distance the projectile can travel before deactivating.
    /// </summary>
    public ProjectileBuilder WithMaxDistance(float maxDistance)
    {
        _maxDistance = maxDistance;
        return this;
    }

    /// <summary>
    /// Sets a visual asset for the projectile (for multi-character or animated sprites).
    /// </summary>
    public ProjectileBuilder WithAsset(IAsset asset)
    {
        _asset = asset;
        return this;
    }

    /// <summary>
    /// Builds and returns the configured Projectile instance.
    /// </summary>
    public Projectile Build()
    {
        Projectile projectile = new Projectile
        {
            Speed = _speed,
            Position = _position,
            Velocity = _velocity.Normalized(), // Ensure normalized
            DisplayCharacter = _displayChar,
            Asset = _asset,
            MaxDistance = _maxDistance
        };

        return projectile;
    }
}
