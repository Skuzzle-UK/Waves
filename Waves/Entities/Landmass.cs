using Waves.Assets.BaseAssets;
using Waves.Core.Configuration;
using Waves.Core.Enums;
using Waves.Core.Interfaces;

namespace Waves.Entities;

/// <summary>
/// Represents a scrolling landmass chunk that forms environmental boundaries at the top or bottom of the screen.
/// Landmass chunks create a canyon-like environment and damage the player on collision.
/// </summary>
public class Landmass : BaseEntity
{
    /// <summary>
    /// The width of the game area (used for despawn detection).
    /// </summary>
    private readonly float _gameWidth;

    /// <summary>
    /// Callback to invoke when player collides with this landmass.
    /// </summary>
    private readonly Action<int>? _onPlayerCollision;

    /// <summary>
    /// The vertical lane position of this landmass chunk (Top or Bottom).
    /// </summary>
    public LandmassPosition LanePosition { get; }

    public Landmass(IAsset asset, float speed, float gameWidth, LandmassPosition position, Action<int>? damageCallback)
    {
        Asset = asset;
        Speed = speed;
        _gameWidth = gameWidth;
        LanePosition = position;
        _onPlayerCollision = damageCallback;

        // Configure landmass properties
        Layer = CollisionLayer.Obstacle;
        CollidesWith = CollisionLayer.Player;
        ClampToBounds = false; // Allow scrolling off-screen
        RenderPriority = GameConstants.Landmass.RenderPriority;
        //RenderColor = Spectre.Console.Color.Yellow; // TODO Setting the rendercolour make the landmass buggy.
    }

    /// <summary>
    /// Updates the landmass chunk and auto-deactivates when it scrolls off-screen.
    /// </summary>
    /// <param name="deltaTime">Time elapsed since last update in seconds.</param>
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        // Deactivate landmass when it scrolls completely off the left edge of the screen
        if (Asset != null && Position.X < -Asset.Width)
        {
            IsActive = false;
        }
    }

    /// <summary>
    /// Called when this landmass collides with another entity.
    /// Damage is handled by the Player entity to respect invulnerability timers.
    /// </summary>
    public override void OnCollision(ICollidable other)
    {
        // Collision response is handled by the Player entity
        // This prevents bypassing the player's invulnerability system
    }
}
