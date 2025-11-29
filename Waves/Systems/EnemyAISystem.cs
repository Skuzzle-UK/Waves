using Waves.Core.AI;
using Waves.Core.Collision;
using Waves.Core.Configuration;
using Waves.Core.Interfaces;
using Waves.Core.Maths;
using Waves.Entities;
using Waves.Entities.Factories;

namespace Waves.Systems;

/// <summary>
/// System that manages enemy AI behaviors, terrain avoidance, and shooting logic.
/// Updates enemy movement and handles projectile firing.
/// </summary>
public class EnemyAISystem : IUpdatable
{
    private readonly IEntityRegistry _entityRegistry;
    private readonly IEntityFactory _entityFactory;
    private readonly float _gameWidth;
    private readonly float _gameHeight;

    private readonly List<Enemy> _enemies = [];
    private readonly List<Terrain> _terrain = [];
    private Player? _player;
    private readonly Random _random = new();

    /// <summary>
    /// Update order for enemy AI (160: after projectile spawning, before collision).
    /// </summary>
    public int UpdateOrder => GameConstants.UpdateOrder.EnemyAISystem;

    public EnemyAISystem(IEntityRegistry entityRegistry, IEntityFactory entityFactory)
    {
        _entityRegistry = entityRegistry ?? throw new ArgumentNullException(nameof(entityRegistry));
        _entityFactory = entityFactory ?? throw new ArgumentNullException(nameof(entityFactory));

        _gameWidth = AppWrapper.GameAreaWidth;
        _gameHeight = AppWrapper.GameAreaHeight - GameConstants.Display.GameGridHeightOffset;
    }

    /// <summary>
    /// Sets the player reference for AI calculations.
    /// </summary>
    public void SetPlayer(Player? player)
    {
        _player = player;
    }

    /// <summary>
    /// Registers an enemy with the AI system.
    /// </summary>
    public void RegisterEnemy(Enemy enemy)
    {
        if (!_enemies.Contains(enemy))
        {
            _enemies.Add(enemy);
        }
    }

    /// <summary>
    /// Unregisters an enemy from the AI system.
    /// </summary>
    public void UnregisterEnemy(Enemy enemy)
    {
        _enemies.Remove(enemy);
    }

    /// <summary>
    /// Registers terrain for avoidance calculations.
    /// </summary>
    public void RegisterTerrain(Terrain terrain)
    {
        if (!_terrain.Contains(terrain))
        {
            _terrain.Add(terrain);
        }
    }

    /// <summary>
    /// Unregisters terrain from avoidance calculations.
    /// </summary>
    public void UnregisterTerrain(Terrain terrain)
    {
        _terrain.Remove(terrain);
    }

    /// <summary>
    /// Resets the AI system state (called when a new game starts).
    /// </summary>
    public void Reset()
    {
        _enemies.Clear();
        _terrain.Clear();
        _player = null;
    }

    /// <summary>
    /// Updates all enemy AI behaviors, terrain avoidance, and shooting logic.
    /// </summary>
    public void Update()
    {
        // Build context for AI calculations
        EnemyAIContext context = new EnemyAIContext
        {
            Player = _player,
            NearbyTerrain = GetActiveTerrain(),
            GameWidth = _gameWidth,
            GameHeight = _gameHeight
        };

        // Update each active enemy
        for (int i = _enemies.Count - 1; i >= 0; i--)
        {
            Enemy enemy = _enemies[i];

            // Remove inactive/disposed enemies
            if (!enemy.IsActive || enemy.IsDisposed)
            {
                _enemies.RemoveAt(i);
                continue;
            }

            // Skip enemies without AI
            if (enemy.AIBehavior == null)
            {
                continue;
            }

            // Update AI behavior state
            enemy.AIBehavior.Update(enemy, context);

            // Get desired velocity from AI
            Vector2 desiredVelocity = enemy.AIBehavior.GetDesiredVelocity(enemy, context);

            // Check for terrain threats and override velocity if necessary
            if (IsThreatInPath(enemy, context.NearbyTerrain))
            {
                desiredVelocity = CalculateAvoidanceVector(enemy, context.NearbyTerrain);
            }

            // Apply velocity (normalized and scaled by speed)
            if (desiredVelocity.Length > 0)
            {
                enemy.Velocity = desiredVelocity.Normalized();
                enemy.Speed = GameConstants.EnemyAI.PatrolSpeed; // Default speed for moving enemies
            }
            else
            {
                enemy.Velocity = Vector2.Zero;
                enemy.Speed = GameConstants.EnemyAI.StationarySpeed;
            }

            // Update shooting cooldown
            enemy.UpdateShootCooldown(GameConstants.Timing.FixedDeltaTime);

            // Handle shooting
            if (enemy.CanShoot() && enemy.ShootingPattern != null)
            {
                Vector2 targetDirection = CalculateTargetDirection(enemy, context);
                enemy.ShootingPattern.Fire(enemy, targetDirection, _entityFactory);
                enemy.ResetShootCooldown();
            }
        }
    }

    /// <summary>
    /// Gets a read-only list of active terrain obstacles.
    /// </summary>
    private IReadOnlyList<Terrain> GetActiveTerrain()
    {
        // Filter to only active terrain
        return _terrain.Where(t => t.IsActive && !t.IsDisposed).ToList();
    }

    /// <summary>
    /// Checks if there's a terrain threat in the enemy's path.
    /// </summary>
    private bool IsThreatInPath(Enemy enemy, IReadOnlyList<Terrain> terrain)
    {
        if (enemy.Speed <= 0)
        {
            return false; // Stationary enemies don't need avoidance
        }

        // Calculate lookahead distance based on enemy speed
        float lookaheadDistance = enemy.Speed * GameConstants.EnemyAI.ThreatLookaheadTime;

        // Create threat zone bounding box
        BoundingBox threatZone = CalculateThreatZone(enemy, lookaheadDistance);

        // Check for terrain in threat zone
        foreach (Terrain obstacle in terrain)
        {
            if (obstacle.IsActive && obstacle.GetBounds().Intersects(threatZone))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Calculates the threat zone for lookahead collision detection.
    /// </summary>
    private BoundingBox CalculateThreatZone(Enemy enemy, float lookaheadDistance)
    {
        // Get enemy bounds
        BoundingBox enemyBounds = enemy.GetBounds();

        // Extend bounds vertically (since enemies only move on Y axis)
        float minY = Math.Min(enemyBounds.Bottom, enemyBounds.Top - lookaheadDistance);
        float maxY = Math.Max(enemyBounds.Top, enemyBounds.Bottom + lookaheadDistance);

        return new BoundingBox
        {
            Left = enemyBounds.Left,
            Right = enemyBounds.Right,
            Top = minY,
            Bottom = maxY
        };
    }

    /// <summary>
    /// Calculates an avoidance vector to move away from terrain threats.
    /// </summary>
    private Vector2 CalculateAvoidanceVector(Enemy enemy, IReadOnlyList<Terrain> terrain)
    {
        // Find nearest terrain obstacle
        Terrain? nearestObstacle = null;
        float nearestDistance = float.MaxValue;

        foreach (Terrain obstacle in terrain)
        {
            if (!obstacle.IsActive)
            {
                continue;
            }

            float distance = Vector2.Distance(enemy.Position, obstacle.Position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestObstacle = obstacle;
            }
        }

        if (nearestObstacle == null)
        {
            return Vector2.Zero;
        }

        // Calculate avoidance direction (move away vertically)
        float deltaY = enemy.Position.Y - nearestObstacle.Position.Y;

        // Move away from obstacle on Y axis
        float direction = deltaY > 0 ? 1f : -1f; // Positive if enemy is below obstacle, negative if above

        return new Vector2(0, direction);
    }

    /// <summary>
    /// Calculates the target direction for shooting with inaccuracy spread.
    /// </summary>
    private Vector2 CalculateTargetDirection(Enemy enemy, EnemyAIContext context)
    {
        // If no player, shoot left with spread
        if (context.Player == null)
        {
            return ApplyInaccuracy(Vector2.Left);
        }

        // Calculate leading shot (simple prediction)
        Vector2 toPlayer = context.Player.Position - enemy.Position;
        float distance = toPlayer.Length;

        // Simple time-to-impact calculation
        float projectileSpeed = GameConstants.EnemyProjectile.DefaultSpeed;
        float timeToImpact = distance / projectileSpeed;

        // Predict player position
        Vector2 playerVelocity = context.Player.Velocity * context.Player.Speed;
        Vector2 predictedPosition = context.Player.Position + (playerVelocity * timeToImpact);

        // Calculate direction to predicted position
        Vector2 direction = predictedPosition - enemy.Position;
        Vector2 normalized = direction.Length > 0 ? direction.Normalized() : Vector2.Left;

        // Apply inaccuracy spread
        return ApplyInaccuracy(normalized);
    }

    /// <summary>
    /// Applies random inaccuracy spread to a direction vector.
    /// Adds angular deviation to make shots less perfect.
    /// </summary>
    private Vector2 ApplyInaccuracy(Vector2 direction)
    {
        // Random spread angle in radians (±15 degrees)
        float spreadAngle = ((float)_random.NextDouble() - 0.5f) * 0.52f; // 0.52 radians ≈ 30 degrees total spread

        // Calculate current angle
        float currentAngle = MathF.Atan2(direction.Y, direction.X);

        // Add spread
        float newAngle = currentAngle + spreadAngle;

        // Return new direction with spread applied
        return new Vector2(MathF.Cos(newAngle), MathF.Sin(newAngle));
    }
}
