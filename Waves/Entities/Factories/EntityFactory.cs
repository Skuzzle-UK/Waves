using Waves.Core.Interfaces;
using Waves.Core.Maths;
using Waves.Entities.Builders;
using Waves.Systems;

namespace Waves.Entities.Factories;

/// <summary>
/// Factory for creating all types of game entities with required dependencies.
/// Dependencies are injected via constructor from the DI container.
/// </summary>
public class EntityFactory : IEntityFactory
{
    private readonly InputSystem _inputSystem;
    private readonly IEntityRegistry _entityRegistry;
    private readonly ProjectileSpawner _projectileSpawner;

    /// <summary>
    /// Creates a new entity factory with dependencies injected from DI.
    /// </summary>
    public EntityFactory(
        InputSystem inputSystem,
        IEntityRegistry entityRegistry,
        ProjectileSpawner projectileSpawner)
    {
        _inputSystem = inputSystem;
        _entityRegistry = entityRegistry;
        _projectileSpawner = projectileSpawner;
    }

    /// <summary>
    /// Creates a fully initialized player at the specified position.
    /// </summary>
    public Player CreatePlayer(Vector2 position)
    {
        var player = new Player(position);
        player.Initialize(_inputSystem, _entityRegistry, _projectileSpawner);
        return player;
    }

    /// <summary>
    /// Creates a projectile at the specified position.
    /// Note: Currently projectiles are created through ProjectileSpawner,
    /// but this method provides a unified interface for future refactoring.
    /// </summary>
    public Projectile CreateProjectile(Vector2 position, Vector2 direction)
    {
        // For now, delegate to the existing ProjectileBuilder pattern
        // This can be refactored later to consolidate projectile creation
        return ProjectileBuilder.Create()
            .WithPosition(position)
            .WithDirection(direction)
            .Build();
    }

    // Future enemy creation
    // public BaseEntity CreateEnemy(Vector2 position, EnemyType enemyType)
    // {
    //     return enemyType switch
    //     {
    //         EnemyType.Basic => new BasicEnemy(position),
    //         EnemyType.Fast => new FastEnemy(position),
    //         EnemyType.Heavy => new HeavyEnemy(position),
    //         _ => throw new ArgumentException($"Unknown enemy type: {enemyType}")
    //     };
    // }

    // Future obstacle creation
    // public BaseEntity CreateObstacle(Vector2 position, ObstacleType obstacleType)
    // {
    //     return obstacleType switch
    //     {
    //         ObstacleType.Wall => new WallObstacle(position),
    //         ObstacleType.Destructible => new DestructibleObstacle(position),
    //         _ => throw new ArgumentException($"Unknown obstacle type: {obstacleType}")
    //     };
    // }
}