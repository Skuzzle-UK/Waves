using Waves.Assets.BaseAssets;
using Waves.Core.Configuration;
using Waves.Core.Enums;
using Waves.Core.Interfaces;
using Waves.Core.Maths;

namespace Waves.Entities;

/// <summary>
/// Represents a terrain object (island or boat) that scrolls across the screen.
/// Terrain objects act as interactive obstacles that can collide with players and enemies.
/// </summary>
public class Terrain : BaseEntity
{
    /// <summary>
    /// The width of the game area (used for despawn detection).
    /// </summary>
    private readonly float _gameWidth;

    public Terrain(IAsset asset, float speed, float gameWidth)
    {
        Asset = asset;
        Speed = speed;
        _gameWidth = gameWidth;

        // Configure terrain properties
        Layer = CollisionLayer.Obstacle;
        CollidesWith = CollisionLayer.Player | CollisionLayer.Enemy;
        ClampToBounds = false; // Allow scrolling off-screen
        RenderPriority = GameConstants.Terrain.RenderPriority;
    }

    /// <summary>
    /// Updates the terrain object and auto-deactivates when it scrolls off-screen.
    /// </summary>
    /// <param name="deltaTime">Time elapsed since last update in seconds.</param>
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        // Deactivate terrain when it scrolls completely off the left edge of the screen
        if (Asset != null && Position.X < -Asset.Width)
        {
            IsActive = false;
        }
    }

    /// <summary>
    /// Called when this terrain collides with another entity.
    /// Override to implement specific collision behavior if needed.
    /// </summary>
    public override void OnCollision(ICollidable other)
    {
        // Default: terrain is a passive obstacle with no specific collision response
        // Player and enemies will handle their own collision responses
    }
}
