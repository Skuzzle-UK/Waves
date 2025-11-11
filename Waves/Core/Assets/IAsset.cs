using Waves.Core.Maths;

namespace Waves.Core.Assets;

/// <summary>
/// Base interface for all visual assets that can be rendered in the game.
/// </summary>
public interface IAsset
{
    /// <summary>
    /// Gets the width of the asset in characters.
    /// </summary>
    int Width { get; }

    /// <summary>
    /// Gets the height of the asset in characters.
    /// </summary>
    int Height { get; }

    /// <summary>
    /// Gets the character at the specified position within the asset.
    /// </summary>
    /// <param name="x">X position within the asset (0 to Width-1).</param>
    /// <param name="y">Y position within the asset (0 to Height-1).</param>
    /// <returns>The character at the specified position, or space if out of bounds.</returns>
    char GetCharAt(int x, int y);

    /// <summary>
    /// Gets the center point of the asset for positioning.
    /// </summary>
    Vector2 Center { get; }

    /// <summary>
    /// Updates the asset for animation (if applicable).
    /// </summary>
    /// <param name="deltaTime">Time since last update in seconds.</param>
    void Update(float deltaTime);
}