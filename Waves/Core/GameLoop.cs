using Microsoft.Extensions.Hosting;
using Waves.Core.Enums;
using Waves.Core.Interfaces;

namespace Waves.Core;

/// <summary>
/// Provides a game tick mechanism for time-based updates and animations.
/// Integrates with GameStateManager to automatically pause/resume based on game state.
/// Implements both event-based subscription and ordered IUpdatable system patterns.
/// </summary>
public class GameLoop : IGameLoop, IHostedService, IDisposable
{
    private readonly int _tickRateMilliseconds;
    private readonly IGameStateManager _gameStateManager;
    private readonly object _lock = new();
    private readonly List<IUpdatable> _updatableSystems = [];

    private Timer? _timer;
    private bool _isPaused;
    private bool _isDisposed;

    /// <inheritdoc/>
    public event Action? OnTick;

    /// <inheritdoc/>
    public bool IsRunning => _timer != null && !_isPaused;

    /// <summary>
    /// Initializes a new instance of the GameTick class.
    /// </summary>
    /// <param name="tickRateMilliseconds">The interval between ticks in milliseconds.</param>
    /// <param name="gameStateManager">The game state manager for automatic pause/resume integration.</param>
    public GameLoop(int tickRateMilliseconds, IGameStateManager gameStateManager)
    {
        _tickRateMilliseconds = tickRateMilliseconds;
        _gameStateManager = gameStateManager;

        // Subscribe to game state changes for automatic pause/resume
        _gameStateManager.GameStateChanged += OnGameStateChanged;
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
    /// Handles game state changes to automatically pause/resume the tick system.
    /// </summary>
    private void OnGameStateChanged(object? sender, GameStates newState)
    {
        switch (newState)
        {
            case GameStates.RUNNING:
                Resume();
                break;
            case GameStates.PAUSED:
                Pause();
                break;
            case GameStates.ENDED:
                Stop();
                break;
            case GameStates.PREPARING:
                // Keep current state during preparation
                break;
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

        _gameStateManager.GameStateChanged -= OnGameStateChanged;
        Stop();
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }
}
