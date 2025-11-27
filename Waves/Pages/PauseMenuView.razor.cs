using Microsoft.AspNetCore.Components;

namespace Waves.Pages;

public partial class PauseMenuView : ComponentBase
{
    [Parameter]
    public Action? OnResume { get; set; }

    [Parameter]
    public Action? OnMainMenu { get; set; }

    [Parameter]
    public Action? OnQuit { get; set; }

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

    private void OnResumeClicked()
    {
        OnResume?.Invoke();
    }

    private void OnMainMenuClicked()
    {
        OnMainMenu?.Invoke();
    }

    private void OnQuitClicked()
    {
        OnQuit?.Invoke();
    }
}
