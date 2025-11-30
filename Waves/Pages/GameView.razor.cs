using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using Waves.Assets.Audio;
using Waves.Core.Enums;
using Waves.Core.Interfaces;
using Waves.Systems;

namespace Waves.Pages;

public partial class GameView : ComponentBase, IDisposable
{
    [Inject]
    public IGameManager GameManager { get; set; } = null!;

    [Inject]
    public GameRenderService RenderService { get; set; } = null!;

    [Inject]
    public InputSystem InputSystem { get; set; } = null!;

    [Inject]
    public IHostApplicationLifetime ApplicationLifetime { get; set; } = null!;

    [Inject]
    public IAudioManager AudioManager { get; set; } = null!;

    [Parameter]
    public Action<string>? OnNavigate { get; set; }

    [Parameter]
    public int Seed { get; set; }

    private Color _healthBarColour = Color.Green;

    private string _topBorder = string.Empty;
    private string _bottomBorder = string.Empty;

    private int _countdownValue = 3;
    private string _countdownText = "3";
    private System.Threading.Timer? _countdownTimer;
    private bool _isPostBossCountdown = false;

    private SettingsView? _settingsView;
    private bool _showSettings = false;

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
        // Initialize static borders with double horizontal line characters
        int gameWidth = AppWrapper.GameAreaWidth;
        _topBorder = new string('═', gameWidth);
        _bottomBorder = new string('═', gameWidth);

        // Subscribe to events BEFORE starting the game to catch state changes
        RenderService.RenderComplete += ReRenderBlazorView;
        GameManager.GameStateChanged += OnGameStateChanged;

        GameManager.StartNewGame(Seed);
    }

    private void OnGameStateChanged(object? sender, GameStates newState)
    {
        // Start countdown timer when entering countdown state
        if (newState == GameStates.COUNTDOWN)
        {
            // Check if this is after a boss battle (boss battle flag would have been set)
            _isPostBossCountdown = GameManager.IsBossBattle == false && GameManager.Score > 0;
            StartCountdown();
        }

        // Re-render when game state changes to show/hide pause menu
        InvokeAsync(StateHasChanged);
    }

    private void StartCountdown()
    {
        // If this is after a boss battle, show "NICE" first
        if (_isPostBossCountdown)
        {
            _countdownValue = 4;
            _countdownText = "NICE";
            _ = AudioManager.PlayOneShot(AudioResources.SoundEffects.Moo, true);
            // Show "NICE" for 3 seconds, then tick every 1 second
            _countdownTimer = new System.Threading.Timer(CountdownTick, null, 3000, 1000);
        }
        else
        {
            _countdownValue = 3;
            _countdownText = "3";
            _ = AudioManager.PlayOneShot(AudioResources.SoundEffects.Three);
            // Normal countdown: tick every 1 second
            _countdownTimer = new System.Threading.Timer(CountdownTick, null, 1000, 1000);
        }
    }

    private void CountdownTick(object? state)
    {
        _countdownValue--;

        switch (_countdownValue)
        {
            case 3:
                _countdownText = "3";
                _ = AudioManager.PlayOneShot(AudioResources.SoundEffects.Three);
                break;
            case 2:
                _countdownText = "2";
                _ = AudioManager.PlayOneShot(AudioResources.SoundEffects.Two);
                break;
            case 1:
                _countdownText = "1";
                _ = AudioManager.PlayOneShot(AudioResources.SoundEffects.One);
                break;
            case 0:
                _countdownText = "0";
                _ = AudioManager.PlayOneShot(AudioResources.SoundEffects.SurfsUp);
                break;
        }

        if (_countdownValue <= 0)
        {
            _countdownTimer?.Dispose();
            _countdownTimer = null;
            _isPostBossCountdown = false;
            GameManager.StartGameAfterCountdown();
        }

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

    private void RestartGame()
    {
        GameManager.ExitGame();
        GameManager.StartNewGame(Seed);
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
        _countdownTimer?.Dispose();
        _countdownTimer = null;

        RenderService.RenderComplete -= ReRenderBlazorView;
        GameManager.GameStateChanged -= OnGameStateChanged;

        // GameManager handles cleanup
        GameManager.ExitGame();
    }
}
