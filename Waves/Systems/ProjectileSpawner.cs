using Waves.Core.Interfaces;
using Waves.Core.Maths;
using Waves.Entities;
using Waves.Entities.Builders;

namespace Waves.Systems;

/// <summary>
/// System that spawns projectiles when the player presses spacebar.
/// </summary>
public class ProjectileSpawner : IUpdatable
{
    private readonly InputSystem _inputSystem;
    private readonly MovementSystem _movementSystem;
    private readonly GameRenderService _renderService;
    private Player? _player;

    /// <summary>
    /// Update order for projectile spawning (100-199 range: Game logic systems).
    /// </summary>
    public int UpdateOrder => 150;

    public ProjectileSpawner(
        InputSystem inputSystem,
        MovementSystem movementSystem,
        GameRenderService renderService)
    {
        _inputSystem = inputSystem;
        _movementSystem = movementSystem;
        _renderService = renderService;
    }

    /// <summary>
    /// Sets the player reference for projectile spawning.
    /// </summary>
    public void SetPlayer(Player player)
    {
        _player = player;
    }

    /// <summary>
    /// Called each game tick to check for projectile spawning.
    /// </summary>
    public void Update()
    {
        if (_player == null || !_player.IsActive)
        {
            return;
        }

        // Check if spacebar was pressed
        if (_inputSystem.WasSpacePressed())
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
            .WithDirection(Vector2.Right)  // Move to the right
            .WithSpeed(200f)
            .WithDisplayChar('>')
            .WithMaxDistance(1000f)  // Will disappear after traveling off screen
            .Build();

        // Register with systems
        _movementSystem.Register(projectile);
        _renderService.RegisterEntity(projectile);
    }
}
