using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RazorConsole.Core;
using Waves.Core;
using Waves.Core.Interfaces;
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
                services.AddSingleton<IGameLoop>(sp => new GameLoop(16, sp.GetRequiredService<IGameStateManager>())); // 16ms = ~60 FPS
                services.AddSingleton<MovementSystem>();
                services.AddSingleton<InputSystem>();
                services.AddSingleton<GameRenderService>(sp => new GameRenderService(AppWrapper.GameAreaWidth, AppWrapper.GameAreaHeight - 3));
                services.AddSingleton<ScoreSystem>(sp => new ScoreSystem(sp.GetRequiredService<IGameStateManager>()));
                services.AddSingleton<ProjectileSpawner>(sp => new ProjectileSpawner(
                    sp.GetRequiredService<InputSystem>(),
                    sp.GetRequiredService<MovementSystem>(),
                    sp.GetRequiredService<GameRenderService>()
                ));

                services.AddHostedService<GameLoop>(sp => (GameLoop)sp.GetRequiredService<IGameLoop>());
            })
            .UseRazorConsole<AppWrapper>();

        IHost host = hostBuilder.Build();

        // All game systems are registered with GameLoop when the game starts (in Game.razor.cs)
        // to avoid running during MainMenu

        await host.RunAsync();
    }
}
