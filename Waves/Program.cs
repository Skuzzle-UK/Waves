using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RazorConsole.Core;
using Waves.Core;
using Waves.Core.Interfaces;
using Waves.Pages;

namespace Waves;

internal class Program
{
    static async Task Main(string[] args)
    {
        IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddSingleton<IGameStateManager, GameStateManager>();
                services.AddSingleton<IKeyboardService, KeyboardService>();
                services.AddSingleton<ICursorVisibilityService, CursorVisibilityService>();

                services.AddHostedService<KeyboardService>(sp => (KeyboardService)sp.GetRequiredService<IKeyboardService>());
                services.AddHostedService<CursorVisibilityService>(sp => (CursorVisibilityService)sp.GetRequiredService<ICursorVisibilityService>());
            })
            .UseRazorConsole<MainMenu>();

        IHost host = hostBuilder.Build();
        await host.RunAsync();
    }
}
