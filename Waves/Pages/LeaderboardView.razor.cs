using Microsoft.AspNetCore.Components;
using Waves.Services;

namespace Waves.Pages;

public partial class LeaderboardView : ComponentBase
{
    [Inject]
    private ILeaderboardService LeaderboardService { get; set; } = null!;

    [Parameter]
    public EventCallback OnVisibilityChanged { get; set; }

    public bool IsVisible { get; set; }

    private List<LeaderboardEntry> _entries = new();
    private bool _isLoading = true;
    private bool _loadError = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadLeaderboardAsync();
    }

    public async Task Show()
    {
        IsVisible = true;
        await LoadLeaderboardAsync();
        await OnVisibilityChanged.InvokeAsync();
    }

    public async Task Back()
    {
        IsVisible = false;
        await OnVisibilityChanged.InvokeAsync();
    }

    private async Task LoadLeaderboardAsync()
    {
        _isLoading = true;
        _loadError = false;
        await InvokeAsync(StateHasChanged);

        try
        {
            _entries = await LeaderboardService.GetTopScoresAsync(8);
        }
        catch
        {
            _loadError = true;
            _entries = new();
        }
        finally
        {
            _isLoading = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    private string GetLeaderboardRow(LeaderboardEntry entry)
    {
        string paddedName = entry.Name.Length > 40
            ? entry.Name.Substring(0, 40)
            : entry.Name.PadRight(40);

        string paddedPosition = entry.Position.ToString().PadLeft(2);
        string paddedScore = entry.Score.ToString().PadLeft(5);

        return $"║ {paddedPosition} ║ {paddedName} ║ {paddedScore} ║";
    }
}
