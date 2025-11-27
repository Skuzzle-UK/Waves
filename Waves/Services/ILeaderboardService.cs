namespace Waves.Services;

/// <summary>
/// Service for interacting with the online leaderboard API.
/// </summary>
public interface ILeaderboardService
{
    /// <summary>
    /// Submits a score to the leaderboard.
    /// </summary>
    /// <param name="name">Player name.</param>
    /// <param name="score">Score achieved.</param>
    /// <returns>True if submission was successful, false otherwise.</returns>
    Task<bool> AddScoreAsync(string name, int score);

    /// <summary>
    /// Retrieves the top scores from the leaderboard.
    /// </summary>
    /// <param name="limit">Number of top scores to retrieve (default: 5).</param>
    /// <returns>List of leaderboard entries, or empty list if retrieval fails.</returns>
    Task<List<LeaderboardEntry>> GetTopScoresAsync(int limit = 5);
}
