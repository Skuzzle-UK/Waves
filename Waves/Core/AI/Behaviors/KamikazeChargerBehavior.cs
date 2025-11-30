using Waves.Core.Configuration;
using Waves.Core.Maths;
using Waves.Entities;

namespace Waves.Core.AI.Behaviors;

/// <summary>
/// AI behavior for enemies that charge directly toward the player to collide with them.
/// Moves in both X and Y directions to intercept the player.
/// Does not shoot - relies on collision damage instead.
/// </summary>
public class KamikazeChargerBehavior : IAIBehavior
{
    /// <summary>
    /// Updates the kamikaze state (no state to track for this simple implementation).
    /// </summary>
    public void Update(Enemy enemy, EnemyAIContext context)
    {
        // No state needed for simple kamikaze behavior
    }

    /// <summary>
    /// Gets the desired velocity for charging directly toward the player.
    /// </summary>
    public Vector2 GetDesiredVelocity(Enemy enemy, EnemyAIContext context)
    {
        // If no player, don't move
        if (context.Player == null)
        {
            return Vector2.Zero;
        }

        // Calculate direction to player (both X and Y)
        Vector2 directionToPlayer = context.Player.Position - enemy.Position;

        // If already at player position, don't move
        if (directionToPlayer.Length < 1f)
        {
            return Vector2.Zero;
        }

        // Return normalized direction (will be multiplied by enemy speed)
        return directionToPlayer.Normalized();
    }
}
