using Waves.Assets.BaseAssets;
using Waves.Core.Maths;

namespace Waves.Entities.Factories;

/// <summary>
/// Factory interface for creating all types of game entities.
/// </summary>
public interface IEntityFactory
{
    /// <summary>
    /// Creates a fully initialised player at the specified position.
    /// </summary>
    /// <param name="position">The starting position for the player.</param>
    /// <returns>A new player instance with all dependencies configured.</returns>
    Player CreatePlayer(Vector2 position);

    /// <summary>
    /// Creates a projectile at the specified position.
    /// </summary>
    /// <param name="position">The starting position for the projectile.</param>
    /// <param name="direction">The direction the projectile should travel.</param>
    /// <returns>A new projectile instance.</returns>
    Projectile CreateProjectile(Vector2 position, Vector2 direction);
    
    /// <summary>
    /// Creates an enemy at the specified position.
    /// </summary>
    /// <param name="position">The starting position of the enemy.</param>
    /// <param name="asset">The enemy asset.</param>
    /// <returns>A new enemy instance.</returns>
    Enemy CreateEnemy(Vector2 position, IAsset asset);
}