using Waves.Assets.BaseAssets;
using Waves.Core.Enums;
using Waves.Core.Maths;

namespace Waves.Entities;

/// <summary>
/// Represents the animated wave on the left side of the screen.
/// </summary>
public class Wave : BaseEntity
{
    /// <summary>
    /// Render priority for the wave (behind everything else).
    /// </summary>
    public const int WaveRenderPriority = 100;

    /// <summary>
    /// Time in seconds for each animation frame.
    /// </summary>
    public const float FrameTime = 0.5f;

    /// <summary>
    /// Width of the first wave frame.
    /// </summary>
    public const int Frame1Width = 6;

    /// <summary>
    /// Width of the second wave frame.
    /// </summary>
    public const int Frame2Width = 8;

    public Wave(Vector2 position, IAsset asset)
    {
        Position = position;
        Asset = asset;
        Velocity = Vector2.Zero;
        Speed = 0f;
        ClampToBounds = true;
        RenderPriority = WaveRenderPriority;

        // Set collision properties - wave acts as an obstacle that damages the player
        Layer = CollisionLayer.Obstacle;
        CollidesWith = CollisionLayer.Player;
    }

    // No need to override Update - base class handles asset animation
}
