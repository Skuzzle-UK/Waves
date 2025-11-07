using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RazorConsole.Core;
using Waves.Core;
using Waves.Core.Interfaces;
using Waves.Pages;
using Waves.Systems;

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
                services.AddSingleton<IGameLoop>(sp => new GameLoop(16, sp.GetRequiredService<IGameStateManager>())); // 16ms = ~60 FPS
                services.AddSingleton<MovementSystem>();

                services.AddHostedService<KeyboardService>(sp => (KeyboardService)sp.GetRequiredService<IKeyboardService>());
                services.AddHostedService<CursorVisibilityService>(sp => (CursorVisibilityService)sp.GetRequiredService<ICursorVisibilityService>());
                services.AddHostedService<GameLoop>(sp => (GameLoop)sp.GetRequiredService<IGameLoop>());
            })
            .UseRazorConsole<MainMenu>();

        IHost host = hostBuilder.Build();

        // Register MovementSystem with GameTick for ordered updates
        IGameLoop gameTick = host.Services.GetRequiredService<IGameLoop>();
        MovementSystem movementSystem = host.Services.GetRequiredService<MovementSystem>();
        gameTick.RegisterUpdatable(movementSystem);

        await host.RunAsync();
    }
}
