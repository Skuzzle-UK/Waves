using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Waves.Core.Configuration;

namespace Waves.Services;

/// <summary>
/// Service for interacting with the online leaderboard API.
/// </summary>
public class LeaderboardService : ILeaderboardService
{
    private readonly HttpClient _httpClient;

    public LeaderboardService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(GameConstants.Leaderboard.BaseUrl);
    }

    /// <summary>
    /// Submits a score to the leaderboard.
    /// </summary>
    public async Task<bool> AddScoreAsync(string name, int score)
    {
        try
        {
            // Generate hash for validation: SHA256(name + score + secretKey)
            string hash = GenerateHash(name, score);

            string url = $"addScore.php?name={Uri.EscapeDataString(name)}&score={score}&hash={hash}";
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string result = await response.Content.ReadAsStringAsync();
            return result.Contains("successfully");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error adding score: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Retrieves the top scores from the leaderboard.
    /// </summary>
    public async Task<List<LeaderboardEntry>> GetTopScoresAsync(int limit = 5)
    {
        try
        {
            string url = $"displayTop.php?limit={limit}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            if (json == "0" || string.IsNullOrWhiteSpace(json))
            {
                return new List<LeaderboardEntry>();
            }

            List<LeaderboardEntry>? entries = JsonSerializer.Deserialize<List<LeaderboardEntry>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return entries ?? [];
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error retrieving leaderboard: {ex.Message}");
            return [];
        }
    }

    /// <summary>
    /// Generates SHA256 hash for score validation.
    /// Hash format: SHA256(name + score + secretKey)
    /// </summary>
    private string GenerateHash(string name, int score)
    {
        string input = $"{name}{score}{GameConstants.Leaderboard.SecretKey}";
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        byte[] hash = SHA256.HashData(bytes);

        // Convert to hex string
        return Convert.ToHexString(hash).ToLower();
    }
}
