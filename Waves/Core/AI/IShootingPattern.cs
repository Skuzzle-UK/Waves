using Waves.Core.Maths;
using Waves.Entities;
using Waves.Entities.Factories;

namespace Waves.Core.AI;

/// <summary>
/// Interface for enemy shooting patterns.
/// Implementing classes define specific firing behaviors and cooldowns.
/// </summary>
public interface IShootingPattern
{
    /// <summary>
    /// Gets the cooldown time for this shooting pattern.
    /// </summary>
    /// <returns>The cooldown time in seconds.</returns>
    float GetCooldownTime();

    /// <summary>
    /// Fires a projectile using this shooting pattern.
    /// </summary>
    /// <param name="source">The enemy firing the projectile.</param>
    /// <param name="targetDirection">The direction to fire toward.</param>
    /// <param name="factory">Factory for creating projectile entities.</param>
    void Fire(Enemy source, Vector2 targetDirection, IEntityFactory factory);
}
