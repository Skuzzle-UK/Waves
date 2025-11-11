using Waves.Core.Configuration;
using Waves.Core.Interfaces;

namespace Waves.Systems;

/// <summary>
/// System that increments the score over time while the game is running.
/// </summary>
public class ScoreSystem : IUpdatable, IDisposable
{
    private readonly IGameManager _gameManager;
    private float _elapsedTime;

    /// <summary>
    /// Update order for score processing (100-199 range: Game logic systems).
    /// </summary>
    public int UpdateOrder => GameConstants.UpdateOrder.ScoreSystem;

    public ScoreSystem(IGameManager gameManager)
    {
        _gameManager = gameManager;
        _elapsedTime = 0f;

        // Subscribe to game state changes to auto-reset when game is preparing
        _gameManager.GameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(object? sender, Core.Enums.GameStates newState)
    {
        if (newState == Core.Enums.GameStates.PREPARING)
        {
            Reset();
        }
    }

    /// <summary>
    /// Called each game tick to update score timing.
    /// </summary>
    public void Update()
    {
        _elapsedTime += GameConstants.Timing.FixedDeltaTime;

        if (_elapsedTime >= GameConstants.Scoring.ScoreInterval)
        {
            _gameManager.IncrementScore(GameConstants.Scoring.ScoreIncrement);
            _elapsedTime -= GameConstants.Scoring.ScoreInterval; // Keep the remainder to maintain accuracy
        }
    }

    /// <summary>
    /// Resets the elapsed time (call when starting a new game).
    /// </summary>
    public void Reset()
    {
        _elapsedTime = 0f;
    }

    public void Dispose()
    {
        // Unsubscribe from events to prevent memory leaks
        _gameManager.GameStateChanged -= OnGameStateChanged;
    }
}
