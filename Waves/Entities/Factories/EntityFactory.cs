using Waves.Assets.BaseAssets;
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
    /// Creates a fully initialised player at the specified position.
    /// Note: Player must be initialised with damage callback after creation via Initialise().
    /// </summary>
    public Player CreatePlayer(Vector2 position)
    {
        var player = new Player(position);
        // Note: Caller must call player.Initialise() with damage callback
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

    /// <summary>
    /// Creates an enemy at the specified position with the given visual asset.
    /// Enemy is automatically registered with the entity registry.
    /// </summary>
    public Enemy CreateEnemy(Vector2 position, IAsset asset)
    {
        var enemy = EnemyBuilder.Create(_entityRegistry)
            .WithPosition(position)
            .WithAsset(asset)
            .Build();

        // Register the enemy with all relevant systems
        _entityRegistry.RegisterEntity(enemy);

        return enemy;
    }
}