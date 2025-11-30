using Waves.Core.Maths;
using Waves.Entities;

namespace Waves.Core.AI;

/// <summary>
/// Interface for enemy AI behavior strategies.
/// Implementing classes define specific movement and decision-making patterns.
/// </summary>
public interface IAIBehavior
{
    /// <summary>
    /// Updates the enemy's state based on this AI behavior.
    /// </summary>
    /// <param name="enemy">The enemy being controlled.</param>
    /// <param name="context">Shared context containing game state information.</param>
    void Update(Enemy enemy, EnemyAIContext context);

    /// <summary>
    /// Calculates the desired velocity for the enemy based on this AI behavior.
    /// </summary>
    /// <param name="enemy">The enemy being controlled.</param>
    /// <param name="context">Shared context containing game state information.</param>
    /// <returns>The desired velocity vector for the enemy.</returns>
    Vector2 GetDesiredVelocity(Enemy enemy, EnemyAIContext context);
}
