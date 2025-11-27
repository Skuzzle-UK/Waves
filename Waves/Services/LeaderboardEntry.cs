namespace Waves.Services;

/// <summary>
/// Represents a single leaderboard entry.
/// </summary>
public class LeaderboardEntry
{
    public int Position { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Score { get; set; }
    public string Date { get; set; } = string.Empty;
}
