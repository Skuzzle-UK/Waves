using Waves.Core.Configuration;
using Waves.Core.Maths;
using Waves.Entities;
using Waves.Entities.Factories;

namespace Waves.Core.AI.ShootingPatterns;

/// <summary>
/// Rapid fire shooting pattern that fires projectiles at fixed intervals in a straight line.
/// Used by stationary turret enemies.
/// </summary>
public class RapidFirePattern : IShootingPattern
{
    /// <summary>
    /// Gets the cooldown time for rapid fire (short interval for continuous stream).
    /// </summary>
    public float GetCooldownTime() => GameConstants.EnemyAI.RapidFireInterval;

    /// <summary>
    /// Fires a single projectile in the target direction.
    /// </summary>
    public void Fire(Enemy source, Vector2 targetDirection, IEntityFactory factory)
    {
        factory.CreateEnemyProjectile(
            source.Position,
            targetDirection,
            GameConstants.EnemyProjectile.DefaultDamage
        );
    }
}
