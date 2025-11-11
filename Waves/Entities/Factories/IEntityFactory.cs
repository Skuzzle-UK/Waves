using Waves.Core.Maths;

namespace Waves.Entities.Factories;

/// <summary>
/// Factory interface for creating all types of game entities.
/// </summary>
public interface IEntityFactory
{
    /// <summary>
    /// Creates a fully initialized player at the specified position.
    /// </summary>
    /// <param name="position">The starting position for the player.</param>
    /// <returns>A new player instance with all dependencies configured.</returns>
    Player CreatePlayer(Vector2 position);

    /// <summary>
    /// Creates an enemy at the specified position.
    /// </summary>
    /// <param name="position">The starting position for the enemy.</param>
    /// <param name="enemyType">The type of enemy to create.</param>
    /// <returns>A new enemy instance.</returns>
    // BaseEntity CreateEnemy(Vector2 position, EnemyType enemyType);

    /// <summary>
    /// Creates an obstacle at the specified position.
    /// </summary>
    /// <param name="position">The position for the obstacle.</param>
    /// <param name="obstacleType">The type of obstacle to create.</param>
    /// <returns>A new obstacle instance.</returns>
    // BaseEntity CreateObstacle(Vector2 position, ObstacleType obstacleType);

    /// <summary>
    /// Creates a projectile at the specified position.
    /// </summary>
    /// <param name="position">The starting position for the projectile.</param>
    /// <param name="direction">The direction the projectile should travel.</param>
    /// <returns>A new projectile instance.</returns>
    Projectile CreateProjectile(Vector2 position, Vector2 direction);
}