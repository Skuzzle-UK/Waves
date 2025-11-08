namespace Waves.Core.Interfaces;

/// <summary>
/// Provides a game tick mechanism for time-based updates and animations.
/// Integrates with GameStateManager to automatically pause/resume based on game state.
/// </summary>
public interface IGameLoop
{
    /// <summary>
    /// Event fired on each game tick. Subscribers receive notifications at the configured tick rate.
    /// Event handlers execute on a background thread pool thread.
    /// </summary>
    event Action OnTick;

    /// <summary>
    /// Gets a value indicating whether the game tick is currently running (not paused or stopped).
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Subscribes a callback to be invoked on each game tick.
    /// Thread-safe. Can be called from any thread.
    /// </summary>
    /// <param name="callback">The action to invoke on each tick.</param>
    void Subscribe(Action callback);

    /// <summary>
    /// Unsubscribes a callback from tick notifications.
    /// Thread-safe. Can be called from any thread.
    /// </summary>
    /// <param name="callback">The action to remove from tick notifications.</param>
    void Unsubscribe(Action callback);

    /// <summary>
    /// Registers an IUpdatable system to receive ordered update calls.
    /// Systems are invoked in order of their UpdateOrder property (lowest first).
    /// Thread-safe. Can be called from any thread.
    /// </summary>
    /// <param name="updatable">The system to register for updates.</param>
    void RegisterUpdatable(IUpdatable updatable);

    /// <summary>
    /// Unregisters an IUpdatable system from receiving update calls.
    /// Thread-safe. Can be called from any thread.
    /// </summary>
    /// <param name="updatable">The system to unregister.</param>
    void UnregisterUpdatable(IUpdatable updatable);

    /// <summary>
    /// Starts the game tick timer.
    /// Typically called by the hosting infrastructure via IHostedService.StartAsync.
    /// </summary>
    void Start();

    /// <summary>
    /// Stops the game tick timer completely.
    /// Typically called by the hosting infrastructure via IHostedService.StopAsync.
    /// </summary>
    void Stop();

    /// <summary>
    /// Pauses tick notifications without stopping the timer.
    /// Tick events will not fire while paused. Automatically called when game state is PAUSED.
    /// Thread-safe. Can be called from any thread.
    /// </summary>
    void Pause();

    /// <summary>
    /// Resumes tick notifications after being paused.
    /// Automatically called when game state transitions to RUNNING.
    /// Thread-safe. Can be called from any thread.
    /// </summary>
    void Resume();
}
