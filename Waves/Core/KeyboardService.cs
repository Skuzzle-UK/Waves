using Microsoft.Extensions.Hosting;
using Waves.Core.Interfaces;

namespace Waves.Core;

public class KeyboardService : IKeyboardService, IHostedService
{
    private Task? _keyListenerTask;
    private CancellationTokenSource? _cancellationTokenSource;

    public event Action<ConsoleKeyInfo>? OnKeyPressed;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _keyListenerTask = Task.Run(() => ListenForKeys(_cancellationTokenSource.Token), cancellationToken);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_cancellationTokenSource is not null && !_cancellationTokenSource.IsCancellationRequested)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        if (_keyListenerTask != null)
        {
            await _keyListenerTask;
        }
    }

    private void ListenForKeys(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
                OnKeyPressed?.Invoke(keyInfo);
            }
            else
            {
                Thread.Sleep(20);
            }
        }
    }
}
