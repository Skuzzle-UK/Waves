using Waves.Core.Configuration;
using Waves.Core.Maths;
using Waves.Entities;

namespace Waves.Core.AI.Behaviors;

/// <summary>
/// AI behavior for stationary turrets that don't move but shoot at fixed intervals.
/// Simple, predictable threat for players to navigate around.
/// </summary>
public class StationaryTurretBehavior : IAIBehavior
{
    /// <summary>
    /// Updates the enemy's state. Stationary turrets don't change state based on updates.
    /// </summary>
    public void Update(Enemy enemy, EnemyAIContext context)
    {
        // Stationary turrets don't need state updates
    }

    /// <summary>
    /// Gets the desired velocity for a stationary turret (always zero - no movement).
    /// </summary>
    public Vector2 GetDesiredVelocity(Enemy enemy, EnemyAIContext context)
    {
        // Stationary - no movement
        return Vector2.Zero;
    }
}
