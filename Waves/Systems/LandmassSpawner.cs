using Waves.Assets;
using Waves.Assets.BaseAssets;
using Waves.Core.Configuration;
using Waves.Core.Enums;
using Waves.Core.Interfaces;
using Waves.Core.Maths;
using Waves.Entities.Factories;

namespace Waves.Systems;

/// <summary>
/// System that continuously spawns landmass chunks at the top and bottom of the screen.
/// Creates a canyon-like environment with procedurally generated gaps for navigation.
/// Scrolling speed is synchronized with AudioManager.LoopSpeed.
/// </summary>
public class LandmassSpawner : IUpdatable
{
    private readonly IEntityFactory _entityFactory;
    private readonly IEntityRegistry _entityRegistry;
    private readonly IAudioManager _audioManager;
    private readonly int _gameWidth;
    private readonly int _gameHeight;

    private Random? _random;
    private bool _isInitialized;
    private Action<int>? _damageCallback;

    // Spawning state
    private float _rightmostChunkX;
    private bool _currentlyInGap;
    private float _gapStartX;
    private float _currentGapWidth;
    private float _nextGapCheckDistance;
    private float _distanceSinceLastGap;

    /// <summary>
    /// Update order for landmass spawning (120: before TerrainSpawner at 125).
    /// </summary>
    public int UpdateOrder => GameConstants.Landmass.SpawnerUpdateOrder;

    public LandmassSpawner(
        IEntityFactory entityFactory,
        IEntityRegistry entityRegistry,
        IAudioManager audioManager)
    {
        _entityFactory = entityFactory ?? throw new ArgumentNullException(nameof(entityFactory));
        _entityRegistry = entityRegistry ?? throw new ArgumentNullException(nameof(entityRegistry));
        _audioManager = audioManager ?? throw new ArgumentNullException(nameof(audioManager));

        _gameWidth = AppWrapper.GameAreaWidth;
        _gameHeight = AppWrapper.GameAreaHeight - GameConstants.Display.GameGridHeightOffset;
    }

    /// <summary>
    /// Initializes the landmass spawner with a specific seed for deterministic generation.
    /// </summary>
    /// <param name="seed">The seed value for the random number generator.</param>
    /// <param name="damageCallback">Callback to invoke when player collides with landmass.</param>
    public void Initialize(int seed, Action<int> damageCallback)
    {
        _random = new Random(seed);
        _damageCallback = damageCallback;
        _rightmostChunkX = _gameWidth; // Start at right edge of screen
        _currentlyInGap = false;
        _gapStartX = 0f;
        _currentGapWidth = 0f;
        _distanceSinceLastGap = 0f;
        _isInitialized = true;
    }

    /// <summary>
    /// Called each game tick to continuously spawn landmass chunks.
    /// Fills the screen with chunks while maintaining procedural gaps.
    /// </summary>
    public void Update()
    {
        if (!_isInitialized || _random == null)
        {
            return;
        }

        float scrollSpeed = CalculateScrollSpeed();

        // Move the spawn position left to simulate scrolling
        float scrollDistance = scrollSpeed * GameConstants.Timing.FixedDeltaTime;
        _rightmostChunkX -= scrollDistance;

        // Continuously spawn chunks until screen is filled
        while (_rightmostChunkX < _gameWidth + GameConstants.Landmass.ChunkWidth)
        {
            // Spawn chunk pair (top + bottom)
            SpawnChunkPair(_rightmostChunkX - 10, scrollSpeed);
            _rightmostChunkX += GameConstants.Landmass.ChunkWidth;
        }
    }

    /// <summary>
    /// Spawns a coordinated pair of landmass chunks (one at top, one at bottom).
    /// </summary>
    private void SpawnChunkPair(float xPosition, float speed)
    {
        if (_random == null)
        {
            return;
        }

        // Top lane: Position accounts for asset height (assets are centered on their position)
        IAsset topAsset = GetRandomTopAsset();
        float topY = topAsset.Height / 2f; // Center the asset so top edge is at Y=0
        Vector2 topPosition = new Vector2(xPosition, topY);
        _entityFactory.CreateLandmass(topPosition, topAsset, speed, LandmassPosition.Top, _damageCallback);

        // Bottom lane: Position accounts for asset height (assets are centered on their position)
        IAsset bottomAsset = GetRandomBottomAsset();
        float bottomY = _gameHeight - 1 - (bottomAsset.Height / 2f); // Center the asset so bottom edge is at gameHeight-1
        Vector2 bottomPosition = new Vector2(xPosition, bottomY);
        _entityFactory.CreateLandmass(bottomPosition, bottomAsset, speed, LandmassPosition.Bottom, _damageCallback);
    }

    /// <summary>
    /// Calculates the current scrolling speed based on audio tempo.
    /// </summary>
    private float CalculateScrollSpeed()
    {
        float baseSpeed = GameConstants.Landmass.BaseScrollSpeed;
        float currentTempo = _audioManager.LoopSpeed;
        return baseSpeed * currentTempo;
    }

    /// <summary>
    /// Gets a random top wall asset from the available pool.
    /// </summary>
    private IAsset GetRandomTopAsset()
    {
        if (_random == null)
        {
            return LandmassAssets.TopWall1;
        }

        return LandmassAssets.TopWalls[_random.Next(LandmassAssets.TopWalls.Length)];
    }

    /// <summary>
    /// Gets a random bottom wall asset from the available pool.
    /// </summary>
    private IAsset GetRandomBottomAsset()
    {
        if (_random == null)
        {
            return LandmassAssets.BottomWall1;
        }

        return LandmassAssets.BottomWalls[_random.Next(LandmassAssets.BottomWalls.Length)];
    }
}
