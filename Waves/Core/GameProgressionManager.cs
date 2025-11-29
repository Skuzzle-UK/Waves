using Waves.Core.Interfaces;

namespace Waves.Core;

/// <summary>
/// Manages game progression state including speed multiplier and boss battle status.
/// Acts as a shared state service to avoid circular dependencies between
/// GameManager and spawner systems.
/// </summary>
public class GameProgressionManager : IGameProgressionManager
{
    /// <summary>
    /// Gets or sets the current game speed multiplier (1.0 to 2.0).
    /// </summary>
    public float CurrentSpeed { get; set; } = 1.0f;

    /// <summary>
    /// Gets or sets whether the game is currently in a boss battle.
    /// </summary>
    public bool IsBossBattle { get; set; } = false;
}
