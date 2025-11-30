using Waves.Assets.BaseAssets;
using Waves.Core.Enums;
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
    private readonly IAudioManager _audioManager;

    /// <summary>
    /// Creates a new entity factory with dependencies injected from DI.
    /// </summary>
    public EntityFactory(
        InputSystem inputSystem,
        IEntityRegistry entityRegistry,
        ProjectileSpawner projectileSpawner,
        IAudioManager audioManager)
    {
        _inputSystem = inputSystem;
        _entityRegistry = entityRegistry;
        _projectileSpawner = projectileSpawner;
        _audioManager = audioManager;
    }

    /// <summary>
    /// Creates a fully initialised player at the specified position.
    /// Note: Player must be initialised with damage callback after creation via Initialise().
    /// </summary>
    public Player CreatePlayer(Vector2 position)
    {
        Player player = new Player(position);
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
        Enemy enemy = EnemyBuilder.Create(_entityRegistry)
            .WithPosition(position)
            .WithAsset(asset)
            .Build();

        // Register the enemy with all relevant systems
        _entityRegistry.RegisterEntity(enemy);

        return enemy;
    }

    /// <summary>
    /// Creates a terrain object at the specified position with the given visual asset.
    /// Terrain is automatically registered with the entity registry.
    /// </summary>
    public Terrain CreateTerrain(Vector2 position, IAsset asset, float speed, float gameWidth)
    {
        Terrain terrain = TerrainBuilder.Create(gameWidth)
            .WithPosition(position)
            .WithAsset(asset)
            .WithSpeed(speed)
            .Build();

        // Register the terrain with all relevant systems
        _entityRegistry.RegisterEntity(terrain);

        return terrain;
    }

    /// <summary>
    /// Creates a landmass chunk at the specified position with the given visual asset.
    /// Landmass is automatically registered with the entity registry.
    /// </summary>
    public Landmass CreateLandmass(Vector2 position, IAsset asset, float speed, LandmassPosition lanePosition, Action<int>? damageCallback)
    {
        var landmass = LandmassBuilder.Create(AppWrapper.GameAreaWidth)
            .WithPosition(position)
            .WithAsset(asset)
            .WithSpeed(speed)
            .WithLanePosition(lanePosition)
            .WithDamageCallback(damageCallback)
            .Build();

        // Register the landmass with all relevant systems
        _entityRegistry.RegisterEntity(landmass);

        return landmass;
    }

    /// <summary>
    /// Creates Boss1 at the specified position with the given visual asset.
    /// Boss is automatically registered with the entity registry.
    /// </summary>
    public Boss1 CreateBoss1(Vector2 position, IAsset asset, int maxHealth = 500)
    {
        Boss1 boss = new Boss1(asset, position, maxHealth, _entityRegistry, _audioManager);
        _entityRegistry.RegisterEntity(boss);
        return boss;
    }

    /// <summary>
    /// Creates Boss2 at the specified position with the given visual asset.
    /// Boss is automatically registered with the entity registry.
    /// </summary>
    public Boss2 CreateBoss2(Vector2 position, IAsset asset, int maxHealth = 500)
    {
        Boss2 boss = new Boss2(asset, position, maxHealth, _entityRegistry, _audioManager);
        _entityRegistry.RegisterEntity(boss);
        return boss;
    }

    /// <summary>
    /// Creates Boss3 at the specified position with the given visual asset.
    /// Boss is automatically registered with the entity registry.
    /// </summary>
    public Boss3 CreateBoss3(Vector2 position, IAsset asset, int maxHealth = 500)
    {
        Boss3 boss = new Boss3(asset, position, maxHealth, _entityRegistry, _audioManager);
        _entityRegistry.RegisterEntity(boss);
        return boss;
    }

    /// <summary>
    /// Creates Boss4 at the specified position with the given visual asset.
    /// Boss is automatically registered with the entity registry.
    /// </summary>
    public Boss4 CreateBoss4(Vector2 position, IAsset asset, int maxHealth = 500)
    {
        Boss4 boss = new Boss4(asset, position, maxHealth, _entityRegistry, _audioManager);
        _entityRegistry.RegisterEntity(boss);
        return boss;
    }

    /// <summary>
    /// Creates Boss5 at the specified position with the given visual asset.
    /// Boss is automatically registered with the entity registry.
    /// </summary>
    public Boss5 CreateBoss5(Vector2 position, IAsset asset, int maxHealth = 500)
    {
        Boss5 boss = new Boss5(asset, position, maxHealth, _entityRegistry, _audioManager);
        _entityRegistry.RegisterEntity(boss);
        return boss;
    }

    /// <summary>
    /// Creates Boss6 at the specified position with the given visual asset.
    /// Boss is automatically registered with the entity registry.
    /// </summary>
    public Boss6 CreateBoss6(Vector2 position, IAsset asset, int maxHealth = 500)
    {
        Boss6 boss = new Boss6(asset, position, maxHealth, _entityRegistry, _audioManager);
        _entityRegistry.RegisterEntity(boss);
        return boss;
    }

    /// <summary>
    /// Creates an enemy projectile at the specified position.
    /// Projectile is automatically registered with the entity registry.
    /// </summary>
    public EnemyProjectile CreateEnemyProjectile(Vector2 position, Vector2 direction, int damage, string? soundEffect)
    {
        EnemyProjectile projectile = EnemyProjectileBuilder.Create()
            .WithPosition(position)
            .WithDirection(direction)
            .WithDamage(damage)
            .Build();

        // Register the projectile with all relevant systems
        _entityRegistry.RegisterEntity(projectile);
        if (soundEffect != null)
        {
            _audioManager?.PlayOneShot(soundEffect);
        }

        return projectile;
    }
}