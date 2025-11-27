using Microsoft.AspNetCore.Components;

namespace Waves.Pages;

public partial class GameOverView : ComponentBase
{
    [Parameter]
    public Action? OnRestart { get; set; }

    [Parameter]
    public Action? OnMainMenu { get; set; }

    private void OnRestartClicked()
    {
        OnRestart?.Invoke();
    }

    private void OnMainMenuClicked()
    {
        OnMainMenu?.Invoke();
    }
}
