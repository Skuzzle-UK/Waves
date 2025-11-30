using Waves.Assets.Audio;
using Waves.Core.Configuration;
using Waves.Core.Maths;
using Waves.Entities;
using Waves.Entities.Factories;

namespace Waves.Core.AI.ShootingPatterns;

/// <summary>
/// Burst fire shooting pattern that fires multiple shots in quick succession.
/// Used by vertical patrol enemies.
/// </summary>
public class BurstFirePattern : IShootingPattern
{
    private int _shotsRemaining;
    private bool _inBurst;

    /// <summary>
    /// Gets the cooldown time - short during burst, longer between bursts.
    /// </summary>
    public float GetCooldownTime()
    {
        if (!_inBurst)
        {
            // Starting a new burst
            _inBurst = true;
            _shotsRemaining = GameConstants.EnemyAI.BurstSize - 1; // -1 because we just fired one
            return GameConstants.EnemyAI.BurstFireInterval;
        }
        else if (_shotsRemaining > 0)
        {
            // Continue burst
            _shotsRemaining--;
            return GameConstants.EnemyAI.BurstFireInterval;
        }
        else
        {
            // End burst, long cooldown
            _inBurst = false;
            return GameConstants.EnemyAI.BurstCooldown;
        }
    }

    /// <summary>
    /// Fires a single projectile toward the target direction.
    /// </summary>
    public void Fire(Enemy source, Vector2 targetDirection, IEntityFactory factory)
    {
        factory.CreateEnemyProjectile(
            source.Position,
            targetDirection,
            GameConstants.EnemyProjectile.DefaultDamage,
            AudioResources.SoundEffects.EightBit.Shoot_003
        );
    }
}
