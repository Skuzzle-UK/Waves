namespace Waves.Core.Interfaces;

/// <summary>
/// Manages the current game speed multiplier.
/// Provides a shared service for speed progression that can be used by
/// GameManager (to update) and spawners (to read) without circular dependencies.
/// </summary>
public interface IGameSpeedManager
{
    /// <summary>
    /// Gets the current game speed multiplier (1.0 to 2.0).
    /// </summary>
    float CurrentSpeed { get; set; }
}
