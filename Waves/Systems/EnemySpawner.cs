using Waves.Assets;
using Waves.Assets.BaseAssets;
using Waves.Core.AI.Behaviors;
using Waves.Core.AI.ShootingPatterns;
using Waves.Core.Configuration;
using Waves.Core.Enums;
using Waves.Core.Interfaces;
using Waves.Core.Maths;
using Waves.Entities;
using Waves.Entities.Factories;

namespace Waves.Systems;

/// <summary>
/// System that procedurally spawns enemies with AI at regular intervals.
/// Implements difficulty scaling over time with varying AI type distribution.
/// </summary>
public class EnemySpawner : IUpdatable
{
    private readonly IEntityFactory _entityFactory;
    private readonly IEntityRegistry _entityRegistry;
    private readonly EnemyAISystem _enemyAISystem;
    private readonly int _gameWidth;
    private readonly int _gameHeight;

    private Random? _random;
    private float _spawnTimer;
    private float _nextSpawnInterval;
    private float _gameDuration;
    private bool _isInitialized;
    private Action<int>? _onEnemyKilled;

    // Spawn zone: Right half of screen only
    private readonly float _minSpawnX;
    private readonly float _maxSpawnX;
    private readonly float _minSpawnY;
    private readonly float _maxSpawnY;

    /// <summary>
    /// Update order for enemy spawning (130: after terrain spawning, before projectile spawning).
    /// </summary>
    public int UpdateOrder => GameConstants.UpdateOrder.EnemySpawner;

    public EnemySpawner(
        IEntityFactory entityFactory,
        IEntityRegistry entityRegistry,
        EnemyAISystem enemyAISystem)
    {
        _entityFactory = entityFactory ?? throw new ArgumentNullException(nameof(entityFactory));
        _entityRegistry = entityRegistry ?? throw new ArgumentNullException(nameof(entityRegistry));
        _enemyAISystem = enemyAISystem ?? throw new ArgumentNullException(nameof(enemyAISystem));

        _gameWidth = AppWrapper.GameAreaWidth;
        _gameHeight = AppWrapper.GameAreaHeight - GameConstants.Display.GameGridHeightOffset;

        // Configure spawn zone (right half of screen)
        _minSpawnX = _gameWidth * 0.5f;
        _maxSpawnX = _gameWidth - 10f;
        _minSpawnY = 5f;
        _maxSpawnY = _gameHeight - 5f;
    }

    /// <summary>
    /// Initializes the enemy spawner with a specific seed for deterministic generation.
    /// </summary>
    /// <param name="seed">The seed value for the random number generator.</param>
    /// <param name="onEnemyKilled">Callback to invoke when an enemy is killed (for scoring).</param>
    public void Initialize(int seed, Action<int> onEnemyKilled)
    {
        _random = new Random(seed);
        _spawnTimer = 0f;
        _gameDuration = 0f;
        _nextSpawnInterval = CalculateSpawnInterval();
        _onEnemyKilled = onEnemyKilled;
        _isInitialized = true;
    }

    /// <summary>
    /// Resets the spawner state (called when a new game starts).
    /// </summary>
    public void Reset()
    {
        _spawnTimer = 0f;
        _gameDuration = 0f;
        if (_random != null)
        {
            _nextSpawnInterval = CalculateSpawnInterval();
        }
    }

    /// <summary>
    /// Called each game tick to check for enemy spawning.
    /// </summary>
    public void Update()
    {
        if (!_isInitialized || _random == null)
        {
            return;
        }

        // Track game duration for difficulty scaling
        _gameDuration += GameConstants.Timing.FixedDeltaTime;

        // Increment spawn timer
        _spawnTimer += GameConstants.Timing.FixedDeltaTime;

        // Check if it's time to spawn a new enemy
        if (_spawnTimer >= _nextSpawnInterval)
        {
            SpawnEnemy();
            _spawnTimer = 0f;
            _nextSpawnInterval = CalculateSpawnInterval();
        }
    }

    /// <summary>
    /// Spawns a new enemy at a random position in the right half of the screen.
    /// </summary>
    private void SpawnEnemy()
    {
        if (_random == null)
        {
            return;
        }

        // Random position in right half of screen
        float x = (float)(_random.NextDouble() * (_maxSpawnX - _minSpawnX) + _minSpawnX);
        float y = (float)(_random.NextDouble() * (_maxSpawnY - _minSpawnY) + _minSpawnY);
        Vector2 spawnPosition = new Vector2(x, y);

        // Select AI type based on difficulty scaling
        AIType aiType = SelectAIType();

        // Select asset based on AI type (visual distinction)
        IAsset asset = SelectAssetForType(aiType);

        // Create enemy via factory
        Enemy enemy = CreateEnemyWithAI(spawnPosition, asset, aiType);

        // Subscribe to death event for scoring
        enemy.OnDeath += (sender, args) =>
        {
            _onEnemyKilled?.Invoke(GameConstants.Enemy.ScoreOnKill);
        };
    }

    /// <summary>
    /// Creates an enemy with AI behavior assigned based on type.
    /// </summary>
    private Enemy CreateEnemyWithAI(Vector2 position, IAsset asset, AIType aiType)
    {
        // Create base enemy
        Enemy enemy = _entityFactory.CreateEnemy(position, asset);

        // Assign AI behavior and shooting pattern based on type
        switch (aiType)
        {
            case AIType.StationaryTurret:
                enemy.AIBehavior = new StationaryTurretBehavior();
                enemy.ShootingPattern = new RapidFirePattern();
                enemy.Health = 80;
                break;

            case AIType.VerticalPatrol:
                enemy.AIBehavior = new VerticalPatrolBehavior();
                enemy.ShootingPattern = new BurstFirePattern();
                enemy.Health = 10;
                break;

            case AIType.AggressiveChaser:
                enemy.AIBehavior = new AggressiveChaserBehavior();
                enemy.ShootingPattern = new ChargedShotPattern();
                enemy.Health = 30;
                break;

            case AIType.KamikazeCharger:
                enemy.AIBehavior = new KamikazeChargerBehavior();
                enemy.ShootingPattern = null; // Kamikaze enemies don't shoot
                enemy.Health = 50;
                enemy.Speed = GameConstants.Enemy.DefaultSpeed * 1.5f; // Faster movement
                enemy.ApplyKnockback = true; // Enable knockback physics
                break;
        }

        // Register with AI system
        _enemyAISystem.RegisterEnemy(enemy);

        return enemy;
    }

    /// <summary>
    /// Calculates spawn interval based on game duration (difficulty scaling).
    /// </summary>
    private float CalculateSpawnInterval()
    {
        // Difficulty scales linearly from 0 to 2x over 120 seconds
        float difficulty = Math.Min(_gameDuration / 60f, 2.0f);
        float baseInterval = GameConstants.EnemyAI.BaseSpawnInterval;
        float minInterval = GameConstants.EnemyAI.MinSpawnInterval;

        // Reduce interval as difficulty increases
        float scaledInterval = baseInterval - (difficulty * 1.5f);
        return Math.Max(scaledInterval, minInterval);
    }

    /// <summary>
    /// Selects AI type based on game duration (difficulty progression).
    /// </summary>
    private AIType SelectAIType()
    {
        if (_random == null)
        {
            return AIType.StationaryTurret;
        }

        // Calculate game progress (0-1, where 0 is start and 1 is 90+ seconds)
        float progress = Math.Min(_gameDuration / 90f, 1.0f);
        float roll = (float)_random.NextDouble();

        // Early game (0-30s): 60% Turret, 30% Patrol, 10% Kamikaze, 0% Chaser
        if (progress < 0.33f)
        {
            if (roll < 0.6f)
            {
                return AIType.StationaryTurret;
            }
            else if (roll < 0.9f)
            {
                return AIType.VerticalPatrol;
            }
            else
            {
                return AIType.KamikazeCharger;
            }
        }
        // Mid game (30-90s): 30% Turret, 30% Patrol, 20% Kamikaze, 20% Chaser
        else if (progress < 1.0f)
        {
            if (roll < 0.3f)
            {
                return AIType.StationaryTurret;
            }
            else if (roll < 0.6f)
            {
                return AIType.VerticalPatrol;
            }
            else if (roll < 0.8f)
            {
                return AIType.KamikazeCharger;
            }
            else
            {
                return AIType.AggressiveChaser;
            }
        }
        // Late game (90s+): 20% Turret, 20% Patrol, 30% Kamikaze, 30% Chaser
        else
        {
            if (roll < 0.2f)
            {
                return AIType.StationaryTurret;
            }
            else if (roll < 0.4f)
            {
                return AIType.VerticalPatrol;
            }
            else if (roll < 0.7f)
            {
                return AIType.KamikazeCharger;
            }
            else
            {
                return AIType.AggressiveChaser;
            }
        }
    }

    /// <summary>
    /// Selects a visual asset based on AI type for player recognition.
    /// </summary>
    private IAsset SelectAssetForType(AIType aiType)
    {
        return aiType switch
        {
            AIType.StationaryTurret => EnemyAssets.WarShip,  // Solid, stationary look
            AIType.VerticalPatrol => EnemyAssets.Shark,        // Mobile, threatening
            AIType.AggressiveChaser => EnemyAssets.Basic,      // Fast, simple
            AIType.KamikazeCharger => EnemyAssets.Kamikaze,    // Explosive, dangerous
            _ => EnemyAssets.WarShip
        };
    }
}
