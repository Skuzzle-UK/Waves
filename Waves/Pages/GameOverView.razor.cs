using Microsoft.AspNetCore.Components;
using Waves.Services;

namespace Waves.Pages;

public partial class GameOverView : ComponentBase
{
    [Inject]
    private ILeaderboardService LeaderboardService { get; set; } = null!;

    [Parameter]
    public int Score { get; set; }

    [Parameter]
    public Action? OnRestart { get; set; }

    [Parameter]
    public Action? OnMainMenu { get; set; }

    private bool _scoreSubmitted = false;
    private bool _isSubmitting = false;
    private bool _submitSuccess = false;
    private string _playerName = string.Empty;
    private int _inputKey = 0;

    private void OnNameChanged(string value)
    {
        if (value.Length > 40)
        {
            _playerName = value.Substring(0, 40);
            _inputKey++;
        }
        else
        {
            _playerName = value;
        }

        StateHasChanged();
    }

    private async Task OnSubmitClicked()
    {
        if (string.IsNullOrWhiteSpace(_playerName))
        {
            // This is a lie so if the player doesn't enter a name we dont bother adding their score but lie to them instead.
            _submitSuccess = true;
            _isSubmitting = false;
            _scoreSubmitted = true;
            await InvokeAsync(StateHasChanged);
            return;
        }

        _isSubmitting = true;
        await InvokeAsync(StateHasChanged);

        try
        {
            _submitSuccess = await LeaderboardService.AddScoreAsync(_playerName, Score);
        }
        catch
        {
            _submitSuccess = false;
        }
        finally
        {
            _isSubmitting = false;
            _scoreSubmitted = true;
            await InvokeAsync(StateHasChanged);
        }
    }

    private void OnRestartClicked()
    {
        OnRestart?.Invoke();
    }

    private void OnMainMenuClicked()
    {
        OnMainMenu?.Invoke();
    }
}
