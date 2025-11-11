using Waves.Core.Assets;
using Waves.Core.Maths;

namespace Waves.Core.Interfaces;

/// <summary>
/// Interface for entities that can be rendered to the game display.
/// </summary>
public interface IRenderable
{
    /// <summary>
    /// The current position of the renderable entity.
    /// </summary>
    Vector2 Position { get; }

    /// <summary>
    /// The character used to display this entity (for backward compatibility).
    /// </summary>
    char DisplayCharacter { get; }

    /// <summary>
    /// The visual asset used to display this entity.
    /// </summary>
    IAsset? Asset { get; }

    /// <summary>
    /// Whether this entity is active and should be rendered.
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// Whether this entity should be clamped to screen bounds.
    /// </summary>
    bool ClampToBounds { get; }

    /// <summary>
    /// Rendering priority for layering (higher values render on top).
    /// </summary>
    int RenderPriority { get; }
}