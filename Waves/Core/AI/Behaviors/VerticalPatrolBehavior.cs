using Waves.Core.Configuration;
using Waves.Core.Maths;
using Waves.Entities;

namespace Waves.Core.AI.Behaviors;

/// <summary>
/// AI behavior for enemies that patrol vertically within a fixed range around their spawn position.
/// Shoots toward the player's position with burst fire.
/// </summary>
public class VerticalPatrolBehavior : IAIBehavior
{
    private float _spawnY;
    private bool _movingUp = true;
    private bool _initialized = false;

    /// <summary>
    /// Updates the patrol state, switching direction when reaching range limits.
    /// </summary>
    public void Update(Enemy enemy, EnemyAIContext context)
    {
        // Initialize spawn position on first update
        if (!_initialized)
        {
            _spawnY = enemy.Position.Y;
            _initialized = true;
        }

        // Check if we've reached patrol boundaries
        float minY = _spawnY - GameConstants.EnemyAI.PatrolRange;
        float maxY = _spawnY + GameConstants.EnemyAI.PatrolRange;

        if (enemy.Position.Y <= minY)
        {
            _movingUp = false; // Hit top, move down (positive Y)
        }
        else if (enemy.Position.Y >= maxY)
        {
            _movingUp = true; // Hit bottom, move up (negative Y)
        }
    }

    /// <summary>
    /// Gets the desired velocity for vertical patrol movement.
    /// </summary>
    public Vector2 GetDesiredVelocity(Enemy enemy, EnemyAIContext context)
    {
        // Move vertically only
        float direction = _movingUp ? -1f : 1f; // Negative Y is up, positive Y is down
        return new Vector2(0, direction);
    }
}
