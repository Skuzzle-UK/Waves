using Waves.Assets;
using Waves.Assets.Audio;
using Waves.Assets.BaseAssets;
using Waves.Core.Configuration;
using Waves.Core.Enums;
using Waves.Core.Interfaces;
using Waves.Core.Maths;
using Waves.Entities;
using Waves.Entities.Factories;
using Waves.Systems;

namespace Waves.Core;

/// <summary>
/// Manages the complete game lifecycle including state, score, and entity orchestration.
/// Combines state management with game logic for unified control.
/// </summary>
public class GameManager : IGameManager
{
    private readonly IEntityFactory _entityFactory;
    private readonly IEntityRegistry _entityRegistry;
    private readonly IAudioManager _audioManager;
    private readonly InputSystem _inputSystem;
    private readonly ProjectileSpawner _projectileSpawner;
    private readonly TerrainSpawner _terrainSpawner;

    // Store game area dimensions for calculating positions
    private readonly int _gameWidth;
    private readonly int _gameHeight;
    private readonly Vector2 _centerPosition;

    // Store current player reference
    private Player? _currentPlayer;

    // State management properties
    public GameStates CurrentGameState { get; private set; }
    public int Score { get; private set; }
    public int Health { get; private set; }

    // Events
    public event EventHandler<GameStates>? GameStateChanged;
    public event EventHandler<int>? ScoreChanged;
    public event EventHandler<int>? HealthChanged;

    public GameManager(
        IEntityFactory entityFactory,
        IEntityRegistry entityRegistry,
        IAudioManager audioManager,
        InputSystem inputSystem,
        ProjectileSpawner projectileSpawner,
        TerrainSpawner terrainSpawner)
    {
        _entityFactory = entityFactory;
        _entityRegistry = entityRegistry;
        _inputSystem = inputSystem;
        _projectileSpawner = projectileSpawner;
        _terrainSpawner = terrainSpawner;

        _gameWidth = AppWrapper.GameAreaWidth;
        _gameHeight = AppWrapper.GameAreaHeight - GameConstants.Display.GameGridHeightOffset;
        _centerPosition = new Vector2(_gameWidth / 2, _gameHeight / 2);

        // Initialise state
        CurrentGameState = GameStates.ENDED;
        Score = GameConstants.Scoring.InitialScore;
        Health = GameConstants.Player.InitialHealth;
        _audioManager = audioManager;

        _audioManager.SetBackgroundTrack(AudioResources.Music.Waves_001);
        _audioManager.StartBackgroundTrack();
    }

    /// <summary>
    /// Initialises a new game session with all necessary entities.
    /// </summary>
    /// <param name="seed">Optional seed for terrain generation. If not provided, uses default seed.</param>
    public void StartNewGame(int? seed = null)
    {
        // Prepare game state
        NewState(GameStates.PREPARING);
        SetScore(GameConstants.Scoring.InitialScore);
        SetHealth(GameConstants.Player.InitialHealth);
        _entityRegistry.ClearAll();

        // Initialize terrain spawner with provided seed or default
        int terrainSeed = seed ?? GameConstants.Terrain.DefaultSeed;
        _terrainSpawner.Initialize(terrainSeed);

        // Create and initialise player entity with damage callback
        _currentPlayer = _entityFactory.CreatePlayer(_centerPosition);
        _currentPlayer.Initialise(_inputSystem, _entityRegistry, _projectileSpawner, TakeDamage);

        // Spawn test enemies - the one and only BRICKWALLs!
        Vector2 wallPosition = new Vector2(_gameWidth - 10, _gameHeight / 2);
        CreateEnemyWithEventSubscription(wallPosition, EnemyAssets.BrickWall);

        CreateEnemyWithEventSubscription(new(_gameWidth - 10, _gameHeight / 2 + 4), EnemyAssets.BrickWall);
        CreateEnemyWithEventSubscription(new(_gameWidth - 10, _gameHeight / 2 + 8), EnemyAssets.BrickWall);
        CreateEnemyWithEventSubscription(new(_gameWidth - 10, _gameHeight / 2 - 4), EnemyAssets.BrickWall);
        CreateEnemyWithEventSubscription(new(_gameWidth - 10, _gameHeight / 2 - 8), EnemyAssets.BrickWall);

        CreateEnemyWithEventSubscription(new(_gameWidth - 16, _gameHeight / 2 + 4), EnemyAssets.BrickWall);
        CreateEnemyWithEventSubscription(new(_gameWidth - 16, _gameHeight / 2 + 8), EnemyAssets.BrickWall);
        CreateEnemyWithEventSubscription(new(_gameWidth - 16, _gameHeight / 2 - 4 ), EnemyAssets.BrickWall);
        CreateEnemyWithEventSubscription(new(_gameWidth - 16, _gameHeight / 2 - 8 ), EnemyAssets.BrickWall);

        // Start the game
        NewState(GameStates.RUNNING);

        // TODO: Perform game logic here like spawning enemies and obstacles.. levels etc
        // Expecting a loop in here that can accept all game states and act upon them accordingly.. i.e. pause should instantiate a pause message.
    }

    /// <summary>
    /// Cleans up the current game session.
    /// </summary>
    public void ExitGame()
    {
        _entityRegistry.ClearAll();
        NewState(GameStates.ENDED);
    }

    /// <summary>
    /// Toggles between paused and running states.
    /// </summary>
    public void TogglePause()
    {
        var newState = CurrentGameState == GameStates.RUNNING
            ? GameStates.PAUSED
            : GameStates.RUNNING;

        NewState(newState);
    }

    /// <summary>
    /// Increments the player's score by the specified value.
    /// </summary>
    public void IncrementScore(int scoreIncrement)
    {
        Score += scoreIncrement;
        ScoreChanged?.Invoke(this, Score);
    }

    /// <summary>
    /// Sets the game into a new state and notifies subscribers.
    /// </summary>
    /// <param name="newState"></param>
    private void NewState(GameStates newState)
    {
        CurrentGameState = newState;
        GameStateChanged?.Invoke(this, CurrentGameState);
    }

    /// <summary>
    /// Sets the score to a specific value and notifies subscribers.
    /// </summary>
    /// <param name="value"></param>
    private void SetScore(int value)
    {
        Score = value;
        ScoreChanged?.Invoke(this, Score);
    }

    /// <summary>
    /// Reduces the player's health by the specified damage amount.
    /// </summary>
    public void TakeDamage(int damage)
    {
        Health = Math.Max(0, Health - damage);
        HealthChanged?.Invoke(this, Health);

        if (Health <= 0)
        {
            // Handle player death - end the game
            NewState(GameStates.ENDED);
        }
    }

    /// <summary>
    /// Increases the player's health by the specified amount, up to the maximum health.
    /// </summary>
    public void Heal(int amount)
    {
        Health = Math.Min(GameConstants.Player.MaxHealth, Health + amount);
        HealthChanged?.Invoke(this, Health);
    }

    /// <summary>
    /// Sets the health to a specific value and notifies subscribers.
    /// </summary>
    /// <param name="value"></param>
    private void SetHealth(int value)
    {
        Health = value;
        HealthChanged?.Invoke(this, Health);
    }

    /// <summary>
    /// Helper method to create an enemy and subscribe to its death event.
    /// Awards points when the enemy is killed.
    /// </summary>
    /// <param name="position">Position to spawn the enemy.</param>
    /// <param name="asset">Visual asset for the enemy.</param>
    private void CreateEnemyWithEventSubscription(Vector2 position, IAsset asset)
    {
        var enemy = _entityFactory.CreateEnemy(position, asset);
        enemy.OnDeath += (sender, e) =>
        {
            IncrementScore(GameConstants.Enemy.ScoreOnKill);
        };
    }
}