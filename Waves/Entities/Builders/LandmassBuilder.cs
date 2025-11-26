using Waves.Assets.BaseAssets;
using Waves.Core.Configuration;
using Waves.Core.Enums;
using Waves.Core.Maths;

namespace Waves.Entities.Builders;

/// <summary>
/// Builder for creating Landmass entities with a fluent API.
/// Provides a flexible way to configure landmass chunks before instantiation.
/// </summary>
public class LandmassBuilder
{
    private readonly float _gameWidth;
    private Vector2 _position = Vector2.Zero;
    private Vector2 _velocity = Vector2.Left; // Default: moving left
    private float _speed = GameConstants.Landmass.BaseScrollSpeed;
    private IAsset? _asset = null;
    private LandmassPosition _lanePosition = LandmassPosition.Top;
    private Action<int>? _damageCallback = null;

    // Private constructor to enforce use of Create() factory method
    private LandmassBuilder(float gameWidth)
    {
        _gameWidth = gameWidth;
    }

    /// <summary>
    /// Creates a new LandmassBuilder instance with required dependencies.
    /// </summary>
    /// <param name="gameWidth">The width of the game area for despawn detection.</param>
    public static LandmassBuilder Create(float gameWidth) => new(gameWidth);

    /// <summary>
    /// Sets the initial position of the landmass chunk.
    /// </summary>
    public LandmassBuilder WithPosition(Vector2 position)
    {
        _position = position;
        return this;
    }

    /// <summary>
    /// Sets the velocity direction of the landmass chunk.
    /// The vector will be normalized automatically.
    /// </summary>
    public LandmassBuilder WithVelocity(Vector2 velocity)
    {
        _velocity = velocity;
        return this;
    }

    /// <summary>
    /// Sets the scrolling speed of the landmass chunk.
    /// Default is GameConstants.Landmass.BaseScrollSpeed.
    /// </summary>
    public LandmassBuilder WithSpeed(float speed)
    {
        _speed = speed;
        return this;
    }

    /// <summary>
    /// Sets the visual asset for the landmass chunk.
    /// </summary>
    public LandmassBuilder WithAsset(IAsset asset)
    {
        _asset = asset;
        return this;
    }

    /// <summary>
    /// Sets the vertical lane position (Top or Bottom).
    /// </summary>
    public LandmassBuilder WithLanePosition(LandmassPosition position)
    {
        _lanePosition = position;
        return this;
    }

    /// <summary>
    /// Sets the damage callback to invoke when player collides with this landmass.
    /// </summary>
    public LandmassBuilder WithDamageCallback(Action<int>? callback)
    {
        _damageCallback = callback;
        return this;
    }

    /// <summary>
    /// Builds and returns a configured Landmass instance.
    /// </summary>
    public Landmass Build()
    {
        if (_asset == null)
        {
            throw new InvalidOperationException("Landmass asset must be set before building.");
        }

        Landmass landmass = new Landmass(_asset, _speed, _gameWidth, _lanePosition, _damageCallback)
        {
            Position = _position,
            Velocity = _velocity.Length > 0 ? _velocity.Normalized() : Vector2.Left
        };

        return landmass;
    }
}
