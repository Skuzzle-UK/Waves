using Waves.Core.Configuration;
using Waves.Core.Maths;
using Waves.Entities;

namespace Waves.Core.AI.Behaviors;

/// <summary>
/// AI behavior for enemies that aggressively chase the player's Y position.
/// Moves vertically to align with the player and fires charged shots when aligned.
/// </summary>
public class AggressiveChaserBehavior : IAIBehavior
{
    private const float AlignmentThreshold = 5f; // Distance threshold for being "aligned" with player

    /// <summary>
    /// Updates the chaser state (no state to track for this simple implementation).
    /// </summary>
    public void Update(Enemy enemy, EnemyAIContext context)
    {
        // No state needed for simple chaser
    }

    /// <summary>
    /// Gets the desired velocity for chasing the player's Y position.
    /// </summary>
    public Vector2 GetDesiredVelocity(Enemy enemy, EnemyAIContext context)
    {
        // If no player, don't move
        if (context.Player == null)
        {
            return Vector2.Zero;
        }

        // Calculate vertical distance to player
        float deltaY = context.Player.Position.Y - enemy.Position.Y;

        // If already aligned with player, don't move
        if (Math.Abs(deltaY) < AlignmentThreshold)
        {
            return Vector2.Zero;
        }

        // Move toward player's Y position
        float direction = deltaY > 0 ? 1f : -1f; // Positive Y is down
        return new Vector2(0, direction);
    }
}
