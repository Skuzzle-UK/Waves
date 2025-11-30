using Waves.Entities;

namespace Waves.Core.AI;

/// <summary>
/// Shared context data passed to AI behaviors for decision-making.
/// Contains references to game state and entities needed for AI calculations.
/// </summary>
public class EnemyAIContext
{
    /// <summary>
    /// Reference to the player entity.
    /// </summary>
    public Player? Player { get; set; }

    /// <summary>
    /// List of nearby terrain obstacles for avoidance calculations.
    /// </summary>
    public IReadOnlyList<Terrain> NearbyTerrain { get; set; } = Array.Empty<Terrain>();

    /// <summary>
    /// Width of the game area.
    /// </summary>
    public float GameWidth { get; set; }

    /// <summary>
    /// Height of the game area.
    /// </summary>
    public float GameHeight { get; set; }
}
