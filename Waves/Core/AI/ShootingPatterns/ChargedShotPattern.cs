using Waves.Core.Configuration;
using Waves.Core.Maths;
using Waves.Entities;
using Waves.Entities.Factories;

namespace Waves.Core.AI.ShootingPatterns;

/// <summary>
/// Charged shot shooting pattern that fires powerful projectiles after a long charge time.
/// Used by aggressive chaser enemies.
/// </summary>
public class ChargedShotPattern : IShootingPattern
{
    /// <summary>
    /// Gets the cooldown time for charged shots (long charge time).
    /// </summary>
    public float GetCooldownTime() => GameConstants.EnemyAI.ChargeTime;

    /// <summary>
    /// Fires a powerful charged projectile at higher speed and damage.
    /// </summary>
    public void Fire(Enemy source, Vector2 targetDirection, IEntityFactory factory)
    {
        // Create a charged projectile with higher damage and speed
        var projectile = factory.CreateEnemyProjectile(
            source.Position,
            targetDirection,
            GameConstants.EnemyProjectile.ChargedDamage
        );

        // Set higher speed for charged shot
        projectile.Speed = GameConstants.EnemyProjectile.FastSpeed;
    }
}
