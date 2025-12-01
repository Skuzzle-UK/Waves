using Microsoft.Extensions.Hosting;
using Waves.Core.Enums;
using Waves.Core.Interfaces;
using Waves.Systems;

namespace Waves.Core;

/// <summary>
/// Provides a game tick mechanism for time-based updates and animations.
/// Integrates with GameStateManager to automatically pause/resume based on game state.
/// Implements both event-based subscription and ordered IUpdatable system patterns.
/// </summary>
public class GameLoop : IGameLoop, IHostedService, IDisposable
{
    private readonly int _tickRateMilliseconds;
    private readonly IGameManager _gameManager;
    private readonly object _lock = new();
    private readonly List<IUpdatable> _updatableSystems = [];

    // Systems that should be registered when game starts
    private readonly InputSystem _inputSystem;
    private readonly MovementSystem _movementSystem;
    private readonly CollisionSystem _collisionSystem;
    private readonly ProjectileSpawner _projectileSpawner;
    private readonly LandmassSpawner _landmassSpawner;
    private readonly TerrainSpawner _terrainSpawner;
    private readonly ScoreSystem _scoreSystem;
    private readonly GameRenderService _renderService;
    private readonly EnemySpawner _enemySpawner;
    private readonly CollectableSpawner _collectableSpawner;
    private readonly EnemyAISystem _enemyAISystem;

    private bool _gameSystemsRegistered = false;

    private Timer? _timer;
    private bool _isPaused;
    private bool _isDisposed;

    /// <inheritdoc/>
    public event Action? OnTick;

    /// <inheritdoc/>
    public bool IsRunning => _timer != null && !_isPaused;

    /// <summary>
    /// Initialises a new instance of the GameTick class.
    /// </summary>
    /// <param name="tickRateMilliseconds">The interval between ticks in milliseconds.</param>
    /// <param name="gameManager">The game manager for automatic pause/resume integration.</param>
    /// <param name="inputSystem">The input system to register during gameplay.</param>
    /// <param name="movementSystem">The movement system to register during gameplay.</param>
    /// <param name="collisionSystem">The collision system to register during gameplay.</param>
    /// <param name="projectileSpawner">The projectile spawner to register during gameplay.</param>
    /// <param name="landmassSpawner">The landmass spawner to register during gameplay.</param>
    /// <param name="terrainSpawner">The terrain spawner to register during gameplay.</param>
    /// <param name="scoreSystem">The score system to register during gameplay.</param>
    /// <param name="renderService">The render service to register during gameplay.</param>
    /// <param name="enemySpawner">The enemy spawner to register during gameplay.</param>
    /// <param name="collectableSpawner">The collectable spawner to register during gameplay.</param>
    /// <param name="enemyAISystem">The enemy AI system to register during gameplay.</param>
    public GameLoop(
        int tickRateMilliseconds,
        IGameManager gameManager,
        InputSystem inputSystem,
        MovementSystem movementSystem,
        CollisionSystem collisionSystem,
        ProjectileSpawner projectileSpawner,
        LandmassSpawner landmassSpawner,
        TerrainSpawner terrainSpawner,
        ScoreSystem scoreSystem,
        GameRenderService renderService,
        EnemySpawner enemySpawner,
        CollectableSpawner collectableSpawner,
        EnemyAISystem enemyAISystem)
    {
        _tickRateMilliseconds = tickRateMilliseconds;
        _gameManager = gameManager;

        _inputSystem = inputSystem;
        _movementSystem = movementSystem;
        _collisionSystem = collisionSystem;
        _projectileSpawner = projectileSpawner;
        _landmassSpawner = landmassSpawner;
        _terrainSpawner = terrainSpawner;
        _scoreSystem = scoreSystem;
        _renderService = renderService;
        _enemySpawner = enemySpawner;
        _collectableSpawner = collectableSpawner;
        _enemyAISystem = enemyAISystem;

        // Subscribe to game state changes for automatic pause/resume
        _gameManager.GameStateChanged += OnGameStateChanged;
    }

    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Start();
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        Stop();
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void Start()
    {
        lock (_lock)
        {
            if (_timer != null)
            {
                return;
            }

            _timer = new Timer(Tick, null, 0, _tickRateMilliseconds);
            _isPaused = false;
        }
    }

    /// <inheritdoc/>
    public void Stop()
    {
        lock (_lock)
        {
            _timer?.Dispose();
            _timer = null;
            _isPaused = false;
        }
    }

    /// <inheritdoc/>
    public void Pause()
    {
        lock (_lock)
        {
            _isPaused = true;
        }
    }

    /// <inheritdoc/>
    public void Resume()
    {
        lock (_lock)
        {
            _isPaused = false;
        }
    }

    /// <inheritdoc/>
    public void Subscribe(Action callback)
    {
        lock (_lock)
        {
            OnTick += callback;
        }
    }

    /// <inheritdoc/>
    public void Unsubscribe(Action callback)
    {
        lock (_lock)
        {
            OnTick -= callback;
        }
    }

    /// <inheritdoc/>
    public void RegisterUpdatable(IUpdatable updatable)
    {
        lock (_lock)
        {
            if (!_updatableSystems.Contains(updatable))
            {
                _updatableSystems.Add(updatable);
                // Sort by UpdateOrder to maintain deterministic execution order
                _updatableSystems.Sort((a, b) => a.UpdateOrder.CompareTo(b.UpdateOrder));
            }
        }
    }

    /// <inheritdoc/>
    public void UnregisterUpdatable(IUpdatable updatable)
    {
        lock (_lock)
        {
            _updatableSystems.Remove(updatable);
        }
    }

    /// <summary>
    /// Handles game state changes to automatically pause/resume the tick system and manage game systems.
    /// </summary>
    private void OnGameStateChanged(object? sender, GameStates newState)
    {
        switch (newState)
        {
            case GameStates.PREPARING:
                // Register game systems and start the timer when preparing a new game
                RegisterGameSystems();
                Start();
                break;

            case GameStates.COUNTDOWN:
                // Pause the game loop during countdown
                Pause();
                break;

            case GameStates.RUNNING:
                Resume();
                break;

            case GameStates.PAUSED:
                Pause();
                break;

            case GameStates.GAME_OVER:
                // Pause the game loop when showing game over screen
                Pause();
                break;

            case GameStates.ENDED:
                // Unregister game systems when game ends
                UnregisterGameSystems();
                Stop();
                break;
        }
    }

    /// <summary>
    /// Registers all game systems with the game loop.
    /// </summary>
    private void RegisterGameSystems()
    {
        if (!_gameSystemsRegistered)
        {
            RegisterUpdatable(_inputSystem);
            RegisterUpdatable(_scoreSystem);
            RegisterUpdatable((IUpdatable)_gameManager);
            RegisterUpdatable(_landmassSpawner);
            RegisterUpdatable(_terrainSpawner);
            RegisterUpdatable(_enemySpawner);
            RegisterUpdatable(_collectableSpawner);
            RegisterUpdatable(_projectileSpawner);
            RegisterUpdatable(_enemyAISystem);
            RegisterUpdatable(_collisionSystem);
            RegisterUpdatable(_movementSystem);
            RegisterUpdatable(_renderService);
            _gameSystemsRegistered = true;
        }
    }

    /// <summary>
    /// Unregisters all game systems from the game loop.
    /// </summary>
    private void UnregisterGameSystems()
    {
        if (_gameSystemsRegistered)
        {
            UnregisterUpdatable(_inputSystem);
            UnregisterUpdatable(_scoreSystem);
            UnregisterUpdatable((IUpdatable)_gameManager);
            UnregisterUpdatable(_landmassSpawner);
            UnregisterUpdatable(_terrainSpawner);
            UnregisterUpdatable(_enemySpawner);
            UnregisterUpdatable(_projectileSpawner);
            UnregisterUpdatable(_enemyAISystem);
            UnregisterUpdatable(_collisionSystem);
            UnregisterUpdatable(_movementSystem);
            UnregisterUpdatable(_renderService);
            _gameSystemsRegistered = false;
        }
    }

    /// <summary>
    /// Internal tick callback invoked by the timer.
    /// Processes ordered IUpdatable systems first, then fires event for subscribers.
    /// </summary>
    private void Tick(object? state)
    {
        // Check pause state without blocking the timer thread
        bool isPaused;
        lock (_lock)
        {
            isPaused = _isPaused;
        }

        if (isPaused)
        {
            return;
        }

        // Create snapshots to avoid holding locks during update execution
        IUpdatable[] updatableSystems;
        Action? onTickEvent;

        lock (_lock)
        {
            updatableSystems = _updatableSystems.ToArray();
            onTickEvent = OnTick;
        }

        // Invoke ordered systems first
        foreach (IUpdatable system in updatableSystems)
        {
            try
            {
                system.Update();
            }
            catch (Exception ex)
            {
                // Log error but continue processing other systems
                Console.Error.WriteLine($"Error updating system {system.GetType().Name}: {ex.Message}");
            }
        }

        // Then invoke event subscribers
        try
        {
            onTickEvent?.Invoke();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error in tick event handler: {ex.Message}");
        }
    }

    /// <summary>
    /// Disposes of the timer and unsubscribes from game state events.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _gameManager.GameStateChanged -= OnGameStateChanged;
        Stop();
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }
}
