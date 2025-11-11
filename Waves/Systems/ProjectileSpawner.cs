using Waves.Core.Configuration;
using Waves.Core.Interfaces;
using Waves.Core.Maths;
using Waves.Entities;
using Waves.Entities.Builders;

namespace Waves.Systems;

/// <summary>
/// System that spawns projectiles when the fire action is triggered.
/// </summary>
public class ProjectileSpawner : IUpdatable
{
    private readonly IEntityRegistry _entityRegistry;
    private IInputProvider? _inputProvider;
    private Player? _player;

    /// <summary>
    /// Update order for projectile spawning (100-199 range: Game logic systems).
    /// </summary>
    public int UpdateOrder => GameConstants.UpdateOrder.ProjectileSpawner;

    public ProjectileSpawner(IEntityRegistry entityRegistry)
    {
        _entityRegistry = entityRegistry ?? throw new ArgumentNullException(nameof(entityRegistry));
    }

    /// <summary>
    /// Sets the player reference for projectile spawning.
    /// </summary>
    public void SetPlayer(Player player)
    {
        _player = player;
    }

    /// <summary>
    /// Sets the input provider for detecting fire actions.
    /// </summary>
    public void SetInputProvider(IInputProvider inputProvider)
    {
        _inputProvider = inputProvider;
    }

    /// <summary>
    /// Called each game tick to check for projectile spawning.
    /// </summary>
    public void Update()
    {
        if (_player == null || !_player.IsActive || _inputProvider == null)
        {
            return;
        }

        // Check if fire action was triggered
        if (_inputProvider.ConsumeAction("fire"))
        {
            SpawnProjectile();
        }
    }

    private void SpawnProjectile()
    {
        if (_player == null)
        {
            return;
        }

        // Create projectile at player's position
        Projectile projectile = ProjectileBuilder.Create()
            .WithPosition(_player.Position)
            .WithDirection(GameConstants.Projectile.DefaultDirection)
            .WithSpeed(GameConstants.Projectile.DefaultSpeed)
            .WithDisplayChar(GameConstants.Projectile.DefaultCharacter)
            .WithMaxDistance(GameConstants.Projectile.MaxDistance)
            .Build();

        // Register with entity registry - it handles all system registrations
        _entityRegistry.RegisterEntity(projectile);
    }
}
