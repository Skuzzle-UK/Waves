using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Hosting;
using Waves.Core.Interfaces;

namespace Waves.Pages;

public partial class MainMenu : IDisposable
{
    [Inject]
    private IHostApplicationLifetime ApplicationLifetime { get; set; } = null!;

    [Inject]
    private IKeyboardService KeyboardService { get; set; } = null!;

    [Inject]
    private ICursorVisibilityService CursorVisibilityService { get; set; } = null!;

    [Parameter]
    public Action<string>? OnNavigateToGame { get; set; }

    protected override void OnInitialized()
    {
        KeyboardService.StopAsync(ApplicationLifetime.ApplicationStopping);
    }

    private void Start()
    {
        OnNavigateToGame?.Invoke("game");
    }

    private void NavigateToLeaderboard()
    {
        if (CursorVisibilityService.IsVisible)
        {
            CursorVisibilityService.HideCursor();
            return;
        }
        
        CursorVisibilityService.ShowCursor();
    }

    private void Quit()
    {
        ApplicationLifetime.StopApplication();
    }

    public void Dispose()
    {
        KeyboardService.StartAsync(ApplicationLifetime.ApplicationStopping);
    }
}
