using Microsoft.AspNetCore.Components;
using Waves.Core.Interfaces;
using Waves.Core.Maths;
using Waves.Entities;
using Waves.Systems;

namespace Waves.Pages;

public partial class GameView : IDisposable
{
    [Inject]
    public IGameStateManager GameStateManager { get; set; } = null!;

    [Inject]
    public GameRenderService RenderService { get; set; } = null!;

    [Inject]
    public InputSystem InputSystem { get; set; } = null!;

    [Inject]
    public IGameLoop GameLoop { get; set; } = null!;

    [Inject]
    public MovementSystem MovementSystem { get; set; } = null!;

    [Inject]
    public ProjectileSpawner ProjectileSpawner { get; set; } = null!;

    [Inject]
    public ScoreSystem ScoreSystem { get; set; } = null!;

    private Timer? _renderTimer;

    private static int _gameGridHeight => AppWrapper.GameAreaHeight - 3;

    protected override void OnInitialized()
    {
        GameStateManager.PrepareGame();
        GameStateManager.StartGame();

        // TODO: I wonder if this should be part of PrepareGame in the GameStateManager?
        ScoreSystem.Reset();
        GameLoop.RegisterUpdatable(InputSystem);
        GameLoop.RegisterUpdatable(ScoreSystem);
        GameLoop.RegisterUpdatable(ProjectileSpawner);
        GameLoop.RegisterUpdatable(MovementSystem);
        GameLoop.RegisterUpdatable(RenderService);

        // Create player at center of game area
        int centerX = AppWrapper.GameAreaWidth / 2;
        int centerY = (AppWrapper.GameAreaHeight - 3) / 2;

        Player player = new Player(
            inputSystem: InputSystem,
            position: new Vector2(centerX, centerY),
            character: '#'
        );

        // Register player with systems
        MovementSystem.Register(player);
        RenderService.RegisterPlayer(player);
        ProjectileSpawner.SetPlayer(player);

        // TODO: Can the render timer be part of the GameLoop or render service instead?
        // Start render timer at 60 FPS (16ms per frame)
        _renderTimer = new Timer(
            callback: _ => InvokeAsync(StateHasChanged),
            state: null,
            dueTime: 0,
            period: 16
        );
    }

    private string GetRowContent(int row)
    {
        return RenderService.GetRowContent(row);
    }

    public void Dispose()
    {
        _renderTimer?.Dispose();

        GameStateManager.EndGame();

        GameLoop.UnregisterUpdatable(InputSystem);
        GameLoop.UnregisterUpdatable(ScoreSystem);
        GameLoop.UnregisterUpdatable(ProjectileSpawner);
        GameLoop.UnregisterUpdatable(MovementSystem);
        GameLoop.UnregisterUpdatable(RenderService);
    }
}
