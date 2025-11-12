using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Hosting;
using Waves.Core.Enums;
using Waves.Core.Interfaces;
using Waves.Systems;

namespace Waves.Pages;

public partial class GameView : IDisposable
{
    [Inject]
    public IGameManager GameManager { get; set; } = null!;

    [Inject]
    public GameRenderService RenderService { get; set; } = null!;

    [Inject]
    public InputSystem InputSystem { get; set; } = null!;

    [Inject]
    public IHostApplicationLifetime ApplicationLifetime { get; set; } = null!;

    protected override void OnInitialized()
    {
        GameManager.StartNewGame();
        RenderService.RenderComplete += ReRenderBlazorView;
        GameManager.GameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(object? sender, GameStates newState)
    {
        // Re-render when game state changes to show/hide pause menu
        InvokeAsync(StateHasChanged);
    }

    private void ReRenderBlazorView()
    {
        // Check for pause input on each render update
        if (InputSystem.WasEscapePressed())
        {
            GameManager.TogglePause();
        }

        InvokeAsync(StateHasChanged);
    }

    private string GetRenderedContent()
    {
        return RenderService.GetRenderedContent();
    }

    private void ResumeGame()
    {
        GameManager.TogglePause();
    }

    private void ExitApplication()
    {
        ApplicationLifetime.StopApplication();
    }

    public void Dispose()
    {
        RenderService.RenderComplete -= ReRenderBlazorView;
        GameManager.GameStateChanged -= OnGameStateChanged;

        // GameManager handles cleanup
        GameManager.ExitGame();
    }
}
