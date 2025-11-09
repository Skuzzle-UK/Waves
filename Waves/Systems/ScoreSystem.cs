using Waves.Core.Interfaces;

namespace Waves.Systems;

/// <summary>
/// System that increments the score over time while the game is running.
/// </summary>
public class ScoreSystem : IUpdatable
{
    private readonly IGameStateManager _gameStateManager;
    private float _elapsedTime;
    private const float ScoreInterval = 2.0f; // 2 seconds
    private const int ScoreIncrement = 10;
    private const float FixedDeltaTime = 0.016f; // 60 FPS

    /// <summary>
    /// Update order for score processing (100-199 range: Game logic systems).
    /// </summary>
    public int UpdateOrder => 100;

    public ScoreSystem(IGameStateManager gameStateManager)
    {
        _gameStateManager = gameStateManager;
        _elapsedTime = 0f;
    }

    /// <summary>
    /// Called each game tick to update score timing.
    /// </summary>
    public void Update()
    {
        _elapsedTime += FixedDeltaTime;

        if (_elapsedTime >= ScoreInterval)
        {
            _gameStateManager.IncrementScore(ScoreIncrement);
            _elapsedTime -= ScoreInterval; // Keep the remainder to maintain accuracy
        }
    }

    /// <summary>
    /// Resets the elapsed time (call when starting a new game).
    /// </summary>
    public void Reset()
    {
        _elapsedTime = 0f;
    }
}
