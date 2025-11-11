using Microsoft.AspNetCore.Components;
using Waves.Core.Interfaces;
using Waves.Systems;

namespace Waves.Pages;

public partial class GameView : IDisposable
{
    [Inject]
    public IGameManager GameManager { get; set; } = null!;

    [Inject]
    public GameRenderService RenderService { get; set; } = null!;

    protected override void OnInitialized()
    {
        GameManager.InitializeNewGame();
        RenderService.RenderComplete += ReRenderBlazorView;
    }

    private void ReRenderBlazorView()
    {
        InvokeAsync(StateHasChanged);
    }

    private string GetRenderedContent()
    {
        return RenderService.GetRenderedContent();
    }

    public void Dispose()
    {
        RenderService.RenderComplete -= ReRenderBlazorView;

        // GameManager handles cleanup
        GameManager.CleanupGame();
    }
}
