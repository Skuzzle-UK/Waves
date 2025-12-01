using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Hosting;
using Waves.Core.Configuration;
using Waves.Core.Interfaces;

namespace Waves.Pages;

public partial class MainMenu : ComponentBase
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
    private LeaderboardView? _leaderboardView;
    private bool _showLeaderboard;

    protected override void OnInitialized()
    {
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
        InvokeAsync(StateHasChanged);
    }

    private void OnLeaderboardVisibilityChanged()
    {
        if (_leaderboardView != null && !_leaderboardView.IsVisible)
        {
            _showLeaderboard = false;
        }
        InvokeAsync(StateHasChanged);
    }

    private void Start()
    {
        // Pass the seed when starting the game
        OnStartWithSeed?.Invoke(_currentSeed);
        OnNavigate?.Invoke("game");
    }

    private void NavigateToLeaderboard()
    {
        _showLeaderboard = true;
    }

    private void Quit()
    {
        ApplicationLifetime.StopApplication();
    }
}
