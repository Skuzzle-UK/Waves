using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Hosting;

namespace Waves.Pages;

public partial class MainMenu
{
    [Inject]
    private IHostApplicationLifetime ApplicationLifetime { get; set; } = null!;

    [Parameter]
    public Action<string>? OnNavigateToGame { get; set; }

    private void Start()
    {
        // Navigate to game - Game.razor.cs will handle all initialization
        OnNavigateToGame?.Invoke("game");
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
