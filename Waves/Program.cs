using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RazorConsole.Core;
using Waves.Core;
using Waves.Core.Configuration;
using Waves.Core.Interfaces;
using Waves.Entities.Factories;
using Waves.Services;
using Waves.Systems;

namespace Waves;

internal class Program
{
    static async Task Main(string[] args)
    {
        // Set console encoding to UTF-8 for proper Unicode character display
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;

        IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                // Register game systems first
                services.AddSingleton<MovementSystem>();
                services.AddSingleton<InputSystem>();
                services.AddSingleton<CollisionSystem>();
                services.AddSingleton<GameRenderService>(sp => new GameRenderService(AppWrapper.GameAreaWidth, AppWrapper.GameAreaHeight - GameConstants.Display.GameGridHeightOffset));
                services.AddSingleton<IEntityRegistry, EntityRegistry>();

                services.AddSingleton<ProjectileSpawner>(sp => new ProjectileSpawner(
                    sp.GetRequiredService<IEntityRegistry>(),
                    sp.GetRequiredService<IAudioManager>()
                ));

                services.AddSingleton<TerrainSpawner>();
                services.AddSingleton<LandmassSpawner>();

                // Register Enemy AI systems
                services.AddSingleton<EnemyAISystem>();
                services.AddSingleton<EnemySpawner>();

                // Register EntityFactory for creating all game entities
                services.AddSingleton<IEntityFactory, EntityFactory>();
                services.AddSingleton<IGameProgressionManager, GameProgressionManager>();
                services.AddSingleton<IGameManager, GameManager>();
                services.AddSingleton<ScoreSystem>(sp => new ScoreSystem(sp.GetRequiredService<IGameManager>()));
                services.AddSingleton<IAudioManager, AudioManager>();
                services.AddHostedService<AudioManager>();
                services.AddHttpClient<ILeaderboardService, LeaderboardService>();
                services.AddSingleton<IGameLoop>(sp => new GameLoop(
                    GameConstants.Timing.TickRateMilliseconds,
                    sp.GetRequiredService<IGameManager>(),
                    sp.GetRequiredService<InputSystem>(),
                    sp.GetRequiredService<MovementSystem>(),
                    sp.GetRequiredService<CollisionSystem>(),
                    sp.GetRequiredService<ProjectileSpawner>(),
                    sp.GetRequiredService<LandmassSpawner>(),
                    sp.GetRequiredService<TerrainSpawner>(),
                    sp.GetRequiredService<ScoreSystem>(),
                    sp.GetRequiredService<GameRenderService>(),
                    sp.GetRequiredService<EnemySpawner>(),
                    sp.GetRequiredService<EnemyAISystem>()));

                services.AddHostedService<GameLoop>(sp => (GameLoop)sp.GetRequiredService<IGameLoop>());
            })
            .UseRazorConsole<AppWrapper>();

        IHost host = hostBuilder.Build();

        // All game systems are registered with GameLoop when the game starts (in Game.razor.cs)
        // to avoid running during MainMenu

        await host.RunAsync();
    }
}
