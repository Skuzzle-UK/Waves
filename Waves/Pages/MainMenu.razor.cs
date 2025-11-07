using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Hosting;
using Waves.Core.Interfaces;
using Waves.Core.Maths;
using Waves.Entities.Builders;
using Waves.Systems;

namespace Waves.Pages;

public partial class MainMenu : IDisposable
{
    [Inject]
    private IHostApplicationLifetime ApplicationLifetime { get; set; } = null!;

    [Inject]
    private IKeyboardService KeyboardService { get; set; } = null!;

    [Inject]
    private ICursorVisibilityService CursorVisibilityService { get; set; } = null!;

    [Inject]
    private MovementSystem MovementSystem { get; set; } = default!;
    
    [Parameter]
    public Action<string>? OnNavigateToGame { get; set; }

    protected override void OnInitialized()
    {
        KeyboardService.StopAsync(ApplicationLifetime.ApplicationStopping);
    }

    private void Start()
    {
        // Start game logic
        
        Entities.Projectile exampleProjectile = ProjectileBuilder.Create()
            .WithSpeed(200)
            .WithPosition(10, 10)
            .WithDirection(Vector2.Right)
            .Build();

        MovementSystem.Register(exampleProjectile);

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
