using Microsoft.Extensions.Hosting;
using Waves.Core.Interfaces;

namespace Waves.Core;

public class CursorVisibilityService : ICursorVisibilityService, IHostedService
{
    private Task? _cursorManagerTask;
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _isVisible = false;

    public bool IsVisible => _isVisible;

    public void ShowCursor()
    {
        _isVisible = true;
    }

    public void HideCursor()
    {
        _isVisible = false;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _cursorManagerTask = Task.Run(() => ManageCursor(_cancellationTokenSource.Token), cancellationToken);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        if (_cursorManagerTask != null)
        {
            await _cursorManagerTask;
        }
    }

    private async Task ManageCursor(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                Console.CursorVisible = _isVisible;
                await Task.Delay(20, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
                break;
            }
        }
    }
}
