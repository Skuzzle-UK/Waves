using Microsoft.AspNetCore.Components;

namespace Waves;

public partial class AppWrapper : ComponentBase
{
    public static int GameAreaHeight => _panelHeight - 2;
    public static int GameAreaWidth => _panelWidth - 4;

    private static int _consoleWidth => Console.WindowWidth;
    private static int _consoleHeight => Console.WindowHeight;

    private static int _panelHeight => _consoleHeight - 3;
    private static int _panelWidth => _consoleWidth;

    private string _currentPage = "menu";

    private void NavigateTo(string page)
    {
        _currentPage = page;
        StateHasChanged();
    }
}
