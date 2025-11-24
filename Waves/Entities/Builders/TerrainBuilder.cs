using Waves.Assets.BaseAssets;
using Waves.Core.Configuration;
using Waves.Core.Maths;

namespace Waves.Entities.Builders;

/// <summary>
/// Builder for creating Terrain entities with a fluent API.
/// Provides a flexible way to configure terrain objects before instantiation.
/// </summary>
public class TerrainBuilder
{
    private readonly float _gameWidth;
    private Vector2 _position = Vector2.Zero;
    private Vector2 _velocity = Vector2.Left; // Default: moving left
    private float _speed = GameConstants.Terrain.MinSpeed;
    private IAsset? _asset = null;

    // Private constructor to enforce use of Create() factory method
    private TerrainBuilder(float gameWidth)
    {
        _gameWidth = gameWidth;
    }

    /// <summary>
    /// Creates a new TerrainBuilder instance with required dependencies.
    /// </summary>
    /// <param name="gameWidth">The width of the game area for despawn detection.</param>
    public static TerrainBuilder Create(float gameWidth) => new(gameWidth);

    /// <summary>
    /// Sets the initial position of the terrain object.
    /// </summary>
    public TerrainBuilder WithPosition(Vector2 position)
    {
        _position = position;
        return this;
    }

    /// <summary>
    /// Sets the velocity direction of the terrain object.
    /// The vector will be normalized automatically.
    /// </summary>
    public TerrainBuilder WithVelocity(Vector2 velocity)
    {
        _velocity = velocity;
        return this;
    }

    /// <summary>
    /// Sets the scrolling speed of the terrain object.
    /// Default is GameConstants.Terrain.MinSpeed.
    /// </summary>
    public TerrainBuilder WithSpeed(float speed)
    {
        _speed = speed;
        return this;
    }

    /// <summary>
    /// Sets the visual asset for the terrain object.
    /// </summary>
    public TerrainBuilder WithAsset(IAsset asset)
    {
        _asset = asset;
        return this;
    }

    /// <summary>
    /// Builds and returns a configured Terrain instance.
    /// </summary>
    public Terrain Build()
    {
        if (_asset == null)
        {
            throw new InvalidOperationException("Terrain asset must be set before building.");
        }

        Terrain terrain = new Terrain(_asset, _speed, _gameWidth)
        {
            Position = _position,
            Velocity = _velocity.Length > 0 ? _velocity.Normalized() : Vector2.Left
        };

        return terrain;
    }
}
