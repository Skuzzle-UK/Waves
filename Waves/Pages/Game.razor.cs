using Microsoft.AspNetCore.Components;
using Waves.Core.Interfaces;

namespace Waves.Pages;

public partial class Game
{
    [Inject]
    public IGameStateManager GameStateManager { get; set; } = null!;

    private Timer? _waveTimer;

    private static int _gameGridHeight => AppWrapper.GameAreaHeight - 3;
    private static int _gameGridWidth => AppWrapper.GameAreaWidth;

    private string[,] GridArray = new string [_gameGridHeight, _gameGridWidth];

    private bool _wavePulse;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        for(int row = 0; row < _gameGridHeight; row++)
        {
            for (int col = 0; col < _gameGridWidth; col++)
            {
                GridArray[row, col] = " ";
            }
        }

        _waveTimer = new Timer(
            callback: _ => RenderWave(),
            state: null,
            dueTime: 200,
            period: 400
        );

        StateHasChanged();
    }

    private string GetRowContent(int row)
    {
        var rowString = string.Empty;

        for (int column = 0; column < _gameGridWidth; column++)
        {
            rowString += GridArray[row, column];
        }
        return rowString;
    }

    private void RenderWave()
    {
        int count = _wavePulse ? 3 : 2;
        for (int row = 0; row < _gameGridHeight; row++)
        {
            for (int col = 0; col < count; col++)
            {
                GridArray[row, col] = "░";
            }

            GridArray[row, count] = "▒";
            GridArray[row, count + 1] = "▓";
            GridArray[row, count + 2] = "█";
            GridArray[row, count + 3] = " ";
        }

        _wavePulse = !_wavePulse;

        StateHasChanged();
    }

    public void Dispose()
    {
        _waveTimer?.Dispose();
    }
}
