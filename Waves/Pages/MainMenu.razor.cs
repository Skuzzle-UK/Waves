using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Hosting;
using Waves.Core;
using Waves.Core.Configuration;
using Waves.Core.Interfaces;

namespace Waves.Pages;

public partial class MainMenu
{
    [Inject]
    private IHostApplicationLifetime ApplicationLifetime { get; set; } = null!;

    [Inject]
    private IGameManager GameManager { get; set; } = default!;

    [Parameter]
    public Action<string>? OnNavigate { get; set; }

    [Parameter]
    public Action<int>? OnStartWithSeed { get; set; }

    private int _currentSeed = GameConstants.Terrain.DefaultSeed;
    private string _seedInput = string.Empty;
    private SettingsView? _settingsView;
    private bool _showSettings;

    protected override void OnInitialized()
    {
        // Initialize with default seed, padded to match button width (15 chars)
        _seedInput = PadSeedForDisplay(_currentSeed.ToString());
    }

    private string PadSeedForDisplay(string seed)
    {
        // Target width is 15 characters to match button width
        const int targetWidth = 15;
        int paddingNeeded = targetWidth - seed.Length;

        if (paddingNeeded <= 0)
        {
            return seed;
        }

        int leftPadding = paddingNeeded / 2;
        int rightPadding = paddingNeeded - leftPadding;

        return new string(' ', leftPadding) + seed + new string(' ', rightPadding);
    }

    private void OnSeedChanged(string value)
    {
        // Remove whitespace padding
        string trimmedValue = value.Trim();

        // Try to parse the seed
        if (int.TryParse(trimmedValue, out int seed))
        {
            _currentSeed = seed;
            _seedInput = PadSeedForDisplay(trimmedValue);
        }
        else if (string.IsNullOrWhiteSpace(trimmedValue))
        {
            // If empty, reset to default
            _currentSeed = GameConstants.Terrain.DefaultSeed;
            _seedInput = PadSeedForDisplay(_currentSeed.ToString());
        }
        else if (int.TryParse(value[^1..], out int lastCharacter))
        {
            _currentSeed = lastCharacter;
            _seedInput = PadSeedForDisplay(lastCharacter.ToString());
        }
        value = "";
        // If invalid, keep the current value (don't update)
    }

    private void ShowSettings()
    {
        _showSettings = true;
    }

    private void OnSettingsVisibilityChanged()
    {
        if (_settingsView != null && !_settingsView.IsVisible)
        {
            _showSettings = false;
        }
        StateHasChanged();
    }

    private void Start()
    {
        // Pass the seed when starting the game
        OnStartWithSeed?.Invoke(_currentSeed);
        OnNavigate?.Invoke("game");
    }

    private void NavigateToLeaderboard()
    {
        // Implement leaderboard navigation if needed
    }

    private void Quit()
    {
        ApplicationLifetime.StopApplication();
    }
}
