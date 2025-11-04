using Waves.Core.Enums;

namespace Waves.Core.Interfaces;

/// <summary>
/// Provides an interface for managing the state and score of a game.
/// </summary>
/// <remarks>The <c>IGameStateManager</c> interface defines methods and properties for controlling game flow,
/// including starting and ending the game, toggling pause state, and managing the player's score. It also includes
/// events for monitoring changes in game state and score.</remarks>
internal interface IGameStateManager
{
    // TODO: Consider adding a PrepareGame method to set up initial game conditions before starting.
    // TODO: Consider moving score management out of here for better separation of concerns.

    /// <summary>
    /// Get the players current score.
    /// </summary>
    public int Score { get; }

    /// <summary>
    /// Gets the current state of the game.
    /// </summary>
    public GameStates CurrentGameState { get; }

    /// <summary>
    /// Occurs when the game state changes.
    /// </summary>
    /// <remarks>Subscribe to this event to be notified whenever the game transitions between different
    /// states.</remarks>
    event EventHandler<GameStates>? GameStateChanged;
    
    /// <summary>
    /// Occurs when the score changes.
    /// </summary>
    /// <remarks>This event is triggered whenever there is a change in the score value.  Subscribers can
    /// handle this event to perform actions based on the updated score.</remarks>
    event EventHandler<int>? ScoreChanged;

    /// <summary>
    /// Initializes and starts the game, setting up necessary resources and state.
    /// </summary>
    /// <remarks>This method should be called once to begin the game. It prepares the game environment and 
    /// transitions the application to the active game state. Ensure all pre-game configurations are completed before
    /// calling this method.</remarks>
    void StartGame();

    /// <summary>
    /// Ends the current game session and performs necessary cleanup operations.
    /// </summary>
    /// <remarks>This method should be called when the game is over to ensure all resources are properly
    /// released. It may trigger events related to game over, and update the game state accordingly.</remarks>
    void EndGame();

    /// <summary>
    /// Toggles the paused state of the game.
    /// </summary>
    /// <remarks>This method switches the game between paused and unpaused states.  It can be used to pause
    /// the game during gameplay or resume it if it is already paused.</remarks>
    void TogglePauseGame();

    /// <summary>
    /// Increments the current score by a specified value.
    /// </summary>
    /// <remarks>This method updates the score by adding the specified increment value. The
    /// increment value is positive to increase the score and negative to reduce the score.</remarks>
    /// <param name="scoreIncrementValue">The value to add to the current score. Can be a positive or negative integer.</param>
    void IncrementScore(int scoreIncrementValue);
}
