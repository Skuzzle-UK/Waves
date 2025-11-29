using Waves.Assets;
using Waves.Assets.BaseAssets;
using Waves.Core.Configuration;
using Waves.Core.Interfaces;
using Waves.Core.Maths;
using Waves.Entities.Factories;

namespace Waves.Systems;

/// <summary>
/// System that procedurally spawns terrain objects (islands and boats) at regular intervals.
/// Uses a seeded random number generator for deterministic terrain generation.
/// Terrain speeds scale with GameProgressionManager.CurrentSpeed.
/// </summary>
public class TerrainSpawner : IUpdatable
{
    private readonly IEntityFactory _entityFactory;
    private readonly IEntityRegistry _entityRegistry;
    private readonly IGameProgressionManager _progressionManager;
    private readonly int _gameWidth;
    private readonly int _gameHeight;

    private Random? _random;
    private float _spawnTimer;
    private float _nextSpawnInterval;
    private bool _isInitialized;

    // Array of all available terrain assets
    private static readonly IAsset[] TerrainAssetPool = new[]
    {
        TerrainAssets.Island1,
        TerrainAssets.Island2,
        TerrainAssets.Island3,
        TerrainAssets.Island4,
        TerrainAssets.Boat1,
        TerrainAssets.Boat2
    };

    /// <summary>
    /// Update order for terrain spawning (125: between ScoreSystem and ProjectileSpawner).
    /// </summary>
    public int UpdateOrder => GameConstants.Terrain.SpawnerUpdateOrder;

    public TerrainSpawner(
        IEntityFactory entityFactory,
        IEntityRegistry entityRegistry,
        IGameProgressionManager progressionManager)
    {
        _entityFactory = entityFactory ?? throw new ArgumentNullException(nameof(entityFactory));
        _entityRegistry = entityRegistry ?? throw new ArgumentNullException(nameof(entityRegistry));
        _progressionManager = progressionManager ?? throw new ArgumentNullException(nameof(progressionManager));

        _gameWidth = AppWrapper.GameAreaWidth;
        _gameHeight = AppWrapper.GameAreaHeight - GameConstants.Display.GameGridHeightOffset;
    }

    /// <summary>
    /// Initializes the terrain spawner with a specific seed for deterministic generation.
    /// </summary>
    /// <param name="seed">The seed value for the random number generator.</param>
    public void Initialize(int seed)
    {
        _random = new Random(seed);
        _spawnTimer = 0f;
        _nextSpawnInterval = GetRandomSpawnInterval();
        _isInitialized = true;
    }

    /// <summary>
    /// Resets the spawner state (called when a new game starts).
    /// </summary>
    public void Reset()
    {
        _spawnTimer = 0f;
        if (_random != null)
        {
            _nextSpawnInterval = GetRandomSpawnInterval();
        }
    }

    /// <summary>
    /// Called each game tick to check for terrain spawning.
    /// Stops spawning during boss battles.
    /// </summary>
    public void Update()
    {
        if (!_isInitialized || _random == null)
        {
            return;
        }

        // Stop spawning terrain during boss battles
        if (_progressionManager.IsBossBattle)
        {
            return;
        }

        // Increment spawn timer
        _spawnTimer += GameConstants.Timing.FixedDeltaTime;

        // Check if it's time to spawn new terrain
        if (_spawnTimer >= _nextSpawnInterval)
        {
            SpawnTerrain();
            _spawnTimer = 0f;
            _nextSpawnInterval = GetRandomSpawnInterval();
        }
    }

    /// <summary>
    /// Spawns a new terrain object at the right edge of the screen.
    /// </summary>
    private void SpawnTerrain()
    {
        if (_random == null)
        {
            return;
        }

        // Select a random terrain asset from the pool
        IAsset selectedAsset = TerrainAssetPool[_random.Next(TerrainAssetPool.Length)];

        // Generate random Y position within game bounds, accounting for asset height
        float minY = selectedAsset.Height / 2f;
        float maxY = _gameHeight - (selectedAsset.Height / 2f);
        float randomY = (float)(_random.NextDouble() * (maxY - minY) + minY);

        // Spawn just off the right edge - position so the left edge is at _gameWidth
        // This way it starts scrolling in immediately and looks natural
        Vector2 spawnPosition = new Vector2(_gameWidth - 1, randomY);

        // Generate random speed within configured range, scaled by current game speed
        float baseRandomSpeed = (float)(_random.NextDouble() *
            (GameConstants.Terrain.MaxSpeed - GameConstants.Terrain.MinSpeed) +
            GameConstants.Terrain.MinSpeed);

        float gameSpeed = _progressionManager.CurrentSpeed;
        float terrainMultiplier = (gameSpeed * 3.0f) - 2.0f;
        float scaledSpeed = baseRandomSpeed * terrainMultiplier;

        // Create and register the terrain entity
        _entityFactory.CreateTerrain(spawnPosition, selectedAsset, scaledSpeed, _gameWidth);
    }

    /// <summary>
    /// Gets a random spawn interval between min and max spawn intervals.
    /// </summary>
    private float GetRandomSpawnInterval()
    {
        if (_random == null)
        {
            return GameConstants.Terrain.MinSpawnInterval;
        }

        return (float)(_random.NextDouble() *
            (GameConstants.Terrain.MaxSpawnInterval - GameConstants.Terrain.MinSpawnInterval) +
            GameConstants.Terrain.MinSpawnInterval);
    }
}
