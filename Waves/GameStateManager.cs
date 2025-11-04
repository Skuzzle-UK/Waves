namespace Waves;

internal class GameStateManager : IGameStateManager
{
    public int Score {get; private set;}
    public GameStates CurrentGameState { get; private set; }

    public event EventHandler<GameStates>? GameStateChanged;
    public event EventHandler<int>? ScoreChanged;

    public GameStateManager()
    {
        Score = 0;
        CurrentGameState = GameStates.ENDED;
    }

    public void PrepareGame()
    {
        Score = 0;
        ScoreChanged?.Invoke(this, Score);
        CurrentGameState = GameStates.PREPARING;
        GameStateChanged?.Invoke(this, CurrentGameState);
    }

    public void StartGame()
    {
        CurrentGameState = GameStates.RUNNING;
        GameStateChanged?.Invoke(this, CurrentGameState);
    }

    public void EndGame()
    {
        CurrentGameState = GameStates.ENDED;
        GameStateChanged?.Invoke(this, CurrentGameState);
    }

    public void TogglePauseGame()
    {
        var newGameState = CurrentGameState == GameStates.RUNNING
            ? GameStates.PAUSED
            : GameStates.RUNNING;

        CurrentGameState = newGameState;

        GameStateChanged?.Invoke(this, CurrentGameState);
    }

    public void IncrementScore(int scoreIncrementValue)
    {
        Score += scoreIncrementValue;
        ScoreChanged?.Invoke(this, Score);
    }
}
