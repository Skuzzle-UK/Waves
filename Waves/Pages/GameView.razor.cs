using Microsoft.AspNetCore.Components;
using Waves.Core.Configuration;
using Waves.Core.Interfaces;
using Waves.Core.Maths;
using Waves.Entities.Factories;
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

    [Inject]
    public IEntityFactory EntityFactory { get; set; } = null!;

    private static int _gameGridHeight => AppWrapper.GameAreaHeight - GameConstants.Display.GameGridHeightOffset;

    private readonly Vector2 _centerPosition;

    public GameView()
    {
        // Calculate center position once during construction
        int gameWidth = AppWrapper.GameAreaWidth;
        int gameHeight = AppWrapper.GameAreaHeight - GameConstants.Display.GameGridHeightOffset;
        _centerPosition = new Vector2(gameWidth / 2, gameHeight / 2);
    }

    protected override void OnInitialized()
    {
        GameStateManager.PrepareGame();
        GameStateManager.StartGame();

        GameLoop.RegisterUpdatable(InputSystem);
        GameLoop.RegisterUpdatable(ScoreSystem);
        GameLoop.RegisterUpdatable(ProjectileSpawner);
        GameLoop.RegisterUpdatable(MovementSystem);
        GameLoop.RegisterUpdatable(RenderService);

        RenderService.RenderComplete += ReRenderBlazorView;

        EntityFactory.CreatePlayer(_centerPosition);
    }

    private void ReRenderBlazorView()
    {
        InvokeAsync(StateHasChanged);
    }

    private string GetRowContent(int row)
    {
        return RenderService.GetRowContent(row);
    }

    public void Dispose()
    {
        RenderService.RenderComplete -= ReRenderBlazorView;

        GameStateManager.EndGame();

        GameLoop.UnregisterUpdatable(InputSystem);
        GameLoop.UnregisterUpdatable(ScoreSystem);
        GameLoop.UnregisterUpdatable(ProjectileSpawner);
        GameLoop.UnregisterUpdatable(MovementSystem);
        GameLoop.UnregisterUpdatable(RenderService);
    }
}
