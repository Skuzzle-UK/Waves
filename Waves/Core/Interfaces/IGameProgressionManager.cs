namespace Waves.Core.Interfaces;

/// <summary>
/// Manages game progression state including speed multiplier and boss battle status.
/// Provides a shared service that can be used by GameManager (to update) and 
/// spawners (to read) without circular dependencies.
/// </summary>
public interface IGameProgressionManager
{
    /// <summary>
    /// Gets the current game speed multiplier (1.0 to 2.0).
    /// </summary>
    float CurrentSpeed { get; set; }

    /// <summary>
    /// Gets or sets whether the game is currently in a boss battle.
    /// </summary>
    bool IsBossBattle { get; set; }
}
