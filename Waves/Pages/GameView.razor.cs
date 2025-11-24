using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
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

    [Parameter]
    public Action<string>? OnNavigate { get; set; }

    [Parameter]
    public int Seed { get; set; }

    private Color _healthBarColour = Color.Green;
    
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
        InvokeAsync(StateHasChanged);
    }

    protected override void OnInitialized()
    {
        GameManager.StartNewGame(Seed);
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

    private string GetHealthBar()
    {
        string bar = "HP: |";
        const int barLength = 20;
        float healthPercent = (float)GameManager.Health / Core.Configuration.GameConstants.Player.MaxHealth;
        
        int filledBlocks = (int)(healthPercent * barLength);
        
        _healthBarColour = healthPercent > 0.6 ? Color.Green : Color.Yellow;
        _healthBarColour = healthPercent > 0.4 ? _healthBarColour : Color.Orange1;
        _healthBarColour = healthPercent > 0.2 ? _healthBarColour : Color.Red;
        
        for (int i = 0; i < barLength; i++)
        {
            if (i < filledBlocks)
            {
                bar += "▓";
            }
            else
            {
                bar += "░";
            }
        }
        bar += "|";

        return bar;
    }

    private void ResumeGame()
    {
        GameManager.TogglePause();
    }

    private void ExitApplication()
    {
        ApplicationLifetime.StopApplication();
    }

    private void ReturnToMainMenu()
    {
        GameManager.ExitGame();
        OnNavigate?.Invoke("menu");
    }

    public void Dispose()
    {
        RenderService.RenderComplete -= ReRenderBlazorView;
        GameManager.GameStateChanged -= OnGameStateChanged;

        // GameManager handles cleanup
        GameManager.ExitGame();
    }
}
