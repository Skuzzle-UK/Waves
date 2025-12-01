using Waves.Assets;
using Waves.Core.Configuration;
using Waves.Core.Enums;
using Waves.Core.Interfaces;
using Waves.Core.Maths;
using Waves.Entities;

namespace Waves.Systems;

/// <summary>
/// System that rarely spawns collectable power-ups for the player.
/// </summary>
public class CollectableSpawner : IUpdatable
{
    private readonly IEntityRegistry _entityRegistry;
    private readonly int _gameWidth;
    private readonly int _gameHeight;

    private Random? _random;
    private float _spawnTimer;
    private float _nextSpawnInterval;
    private bool _isInitialized;

    // Spawn zone configuration
    private readonly float _minSpawnX;
    private readonly float _maxSpawnX;
    private readonly float _minSpawnY;
    private readonly float _maxSpawnY;

    // Collectables spawn very rarely
    private const float MinSpawnInterval = 30f;  // Minimum 30 seconds between spawns
    private const float MaxSpawnInterval = 60f;  // Maximum 60 seconds between spawns

    /// <summary>
    /// Update order for collectable spawning.
    /// </summary>
    public int UpdateOrder => 135; // After enemy spawner

    public CollectableSpawner(IEntityRegistry entityRegistry)
    {
        _entityRegistry = entityRegistry ?? throw new ArgumentNullException(nameof(entityRegistry));

        _gameWidth = AppWrapper.GameAreaWidth;
        _gameHeight = AppWrapper.GameAreaHeight - GameConstants.Display.GameGridHeightOffset;

        // Configure spawn zone (right side of screen, avoiding edges)
        _minSpawnX = _gameWidth * 0.6f;
        _maxSpawnX = _gameWidth - 10f;
        _minSpawnY = 10f;
        _maxSpawnY = _gameHeight - 10f;
    }

    /// <summary>
    /// Initializes the collectable spawner with a specific seed.
    /// </summary>
    public void Initialize(int seed)
    {
        _random = new Random(seed);
        _spawnTimer = 0f;
        _nextSpawnInterval = CalculateNextSpawnInterval();
        _isInitialized = true;
    }

    /// <summary>
    /// Resets the spawner state.
    /// </summary>
    public void Reset()
    {
        _spawnTimer = 0f;
        if (_random != null)
        {
            _nextSpawnInterval = CalculateNextSpawnInterval();
        }
    }

    /// <summary>
    /// Called each game tick to check for collectable spawning.
    /// </summary>
    public void Update()
    {
        if (!_isInitialized || _random == null)
        {
            return;
        }

        // Increment spawn timer
        _spawnTimer += GameConstants.Timing.FixedDeltaTime;

        // Check if it's time to spawn a new collectable
        if (_spawnTimer >= _nextSpawnInterval)
        {
            SpawnCollectable();
            _spawnTimer = 0f;
            _nextSpawnInterval = CalculateNextSpawnInterval();
        }
    }

    /// <summary>
    /// Spawns a random collectable at a random position on the right side of the screen.
    /// </summary>
    private void SpawnCollectable()
    {
        if (_random == null)
        {
            return;
        }

        // Random position in spawn zone
        float x = (float)(_random.NextDouble() * (_maxSpawnX - _minSpawnX) + _minSpawnX);
        float y = (float)(_random.NextDouble() * (_maxSpawnY - _minSpawnY) + _minSpawnY);
        Vector2 spawnPosition = new Vector2(x, y);

        // Randomly choose between health and invulnerability (50/50 chance)
        CollectableType type = _random.Next(2) == 0
            ? CollectableType.HealthUp
            : CollectableType.Invulnerability;

        // Create collectable
        Collectable collectable = new Collectable
        {
            Position = spawnPosition,
            Type = type,
            Velocity = Vector2.Left, // Scroll left like other entities
            Asset = type == CollectableType.HealthUp
                ? CollectableAssets.HealthUp
                : CollectableAssets.Invulnerability
        };

        _entityRegistry.RegisterEntity(collectable);
    }

    /// <summary>
    /// Calculates the next spawn interval (random between min and max).
    /// </summary>
    private float CalculateNextSpawnInterval()
    {
        if (_random == null)
        {
            return MinSpawnInterval;
        }

        return (float)(_random.NextDouble() * (MaxSpawnInterval - MinSpawnInterval) + MinSpawnInterval);
    }
}
