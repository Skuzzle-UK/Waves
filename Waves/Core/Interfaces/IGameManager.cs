using Waves.Core.Enums;

namespace Waves.Core.Interfaces;

/// <summary>
/// Manages the complete game lifecycle including state, score, and entity orchestration.
/// Combines state management with game logic orchestration for a unified game control interface.
/// </summary>
public interface IGameManager
{
    /// <summary>
    /// Gets the current game state.
    /// </summary>
    GameStates CurrentGameState { get; }

    /// <summary>
    /// Gets the current player score.
    /// </summary>
    int Score { get; }

    /// <summary>
    /// Gets the current player health.
    /// </summary>
    int Health { get; }

    /// <summary>
    /// Event fired when the game state changes.
    /// </summary>
    event EventHandler<GameStates>? GameStateChanged;

    /// <summary>
    /// Event fired when the score changes.
    /// </summary>
    event EventHandler<int>? ScoreChanged;

    /// <summary>
    /// Event fired when the player's health changes.
    /// </summary>
    event EventHandler<int>? HealthChanged;

    /// <summary>
    /// Initialises a new game session, creating all necessary entities and starting gameplay.
    /// This includes player creation, initial enemy spawning, and any level setup.
    /// </summary>
    /// <param name="seed">Optional seed for terrain generation. If not provided, uses default seed.</param>
    void StartNewGame(int? seed = null);

    /// <summary>
    /// Starts the game after the countdown completes.
    /// Transitions from COUNTDOWN state to RUNNING state.
    /// </summary>
    void StartGameAfterCountdown();

    /// <summary>
    /// Cleans up the current game session, removing entities and resetting systems.
    /// Called when returning to menu or ending the game.
    /// </summary>
    void ExitGame();

    /// <summary>
    /// Pauses or resumes the current game.
    /// </summary>
    void TogglePause();

    /// <summary>
    /// Increments the player's score by the specified value.
    /// </summary>
    /// <param name="scoreIncrement">The amount to add to the score.</param>
    void IncrementScore(int scoreIncrement);

    /// <summary>
    /// Reduces the player's health by the specified damage amount.
    /// Triggers HealthChanged event and handles player death if health reaches 0.
    /// </summary>
    /// <param name="damage">The amount of damage to apply.</param>
    void TakeDamage(int damage);

    /// <summary>
    /// Increases the player's health by the specified amount, up to the maximum health.
    /// </summary>
    /// <param name="amount">The amount of health to restore.</param>
    void Heal(int amount);
}