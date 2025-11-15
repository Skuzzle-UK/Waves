using Waves.Assets;
using Waves.Assets.Audio;
using Waves.Core.Configuration;
using Waves.Core.Enums;
using Waves.Core.Interfaces;
using Waves.Core.Maths;
using Waves.Entities.Factories;

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

    // Store game area dimensions for calculating positions
    private readonly int _gameWidth;
    private readonly int _gameHeight;
    private readonly Vector2 _centerPosition;

    // State management properties
    public GameStates CurrentGameState { get; private set; }
    public int Score { get; private set; }

    // Events
    public event EventHandler<GameStates>? GameStateChanged;
    public event EventHandler<int>? ScoreChanged;

    public GameManager(
        IEntityFactory entityFactory,
        IEntityRegistry entityRegistry,
        IAudioManager audioManager)
    {
        _entityFactory = entityFactory;
        _entityRegistry = entityRegistry;

        _gameWidth = AppWrapper.GameAreaWidth;
        _gameHeight = AppWrapper.GameAreaHeight - GameConstants.Display.GameGridHeightOffset;
        _centerPosition = new Vector2(_gameWidth / 2, _gameHeight / 2);

        // Initialize state
        CurrentGameState = GameStates.ENDED;
        Score = GameConstants.Scoring.InitialScore;
        _audioManager = audioManager;

        _audioManager.SetBackgroundTrack(AudioResources.Music.Waves_001);
        _audioManager.StartBackgroundTrack();
    }

    /// <summary>
    /// Initializes a new game session with all necessary entities.
    /// </summary>
    public void StartNewGame()
    {
        // Prepare game state
        NewState(GameStates.PREPARING);
        SetScore(GameConstants.Scoring.InitialScore);
        _entityRegistry.ClearAll();

        // Create player entity
        _entityFactory.CreatePlayer(_centerPosition);

        // Spawn test enemy - the one and only BRICKWALL!
        Vector2 wallPosition = new Vector2(_gameWidth - 10, _gameHeight / 2);
        _entityFactory.CreateEnemy(wallPosition, EnemyAssets.BrickWall);

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
}