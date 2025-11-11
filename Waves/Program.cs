using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RazorConsole.Core;
using Waves.Core;
using Waves.Core.Configuration;
using Waves.Core.Interfaces;
using Waves.Entities.Factories;
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
                services.AddSingleton<IGameLoop>(sp => new GameLoop(GameConstants.Timing.TickRateMilliseconds, sp.GetRequiredService<IGameStateManager>()));
                services.AddSingleton<MovementSystem>();
                services.AddSingleton<InputSystem>();
                services.AddSingleton<GameRenderService>(sp => new GameRenderService(AppWrapper.GameAreaWidth, AppWrapper.GameAreaHeight - GameConstants.Display.GameGridHeightOffset));
                services.AddSingleton<ScoreSystem>(sp => new ScoreSystem(sp.GetRequiredService<IGameStateManager>()));

                // Register EntityRegistry
                services.AddSingleton<IEntityRegistry, EntityRegistry>();

                // Update ProjectileSpawner to use EntityRegistry
                services.AddSingleton<ProjectileSpawner>(sp => new ProjectileSpawner(
                    sp.GetRequiredService<IEntityRegistry>()
                ));

                // Register EntityFactory for creating all game entities
                services.AddSingleton<IEntityFactory, EntityFactory>();

                services.AddHostedService<GameLoop>(sp => (GameLoop)sp.GetRequiredService<IGameLoop>());
            })
            .UseRazorConsole<AppWrapper>();

        IHost host = hostBuilder.Build();

        // All game systems are registered with GameLoop when the game starts (in Game.razor.cs)
        // to avoid running during MainMenu

        await host.RunAsync();
    }
}
