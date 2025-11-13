using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Hosting;
using Waves.Core;
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
