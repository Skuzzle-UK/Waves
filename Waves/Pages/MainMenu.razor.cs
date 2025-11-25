using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Hosting;
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

    private SettingsView? _settingsView;
    private bool _showSettings;

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
