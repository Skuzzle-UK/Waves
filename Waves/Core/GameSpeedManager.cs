using Waves.Core.Interfaces;

namespace Waves.Core;

/// <summary>
/// Manages the current game speed multiplier.
/// Acts as a shared state service to avoid circular dependencies between
/// GameManager and spawner systems.
/// </summary>
public class GameSpeedManager : IGameSpeedManager
{
    /// <summary>
    /// Gets or sets the current game speed multiplier (1.0 to 2.0).
    /// </summary>
    public float CurrentSpeed { get; set; } = 1.0f;
}
