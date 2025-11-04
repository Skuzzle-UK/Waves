using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RazorConsole.Core;

namespace Waves;

internal class Program
{
    static async Task Main(string[] args)
    {
        IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddSingleton<IGameStateManager, GameStateManager>();
            })
            .UseRazorConsole<Sample>();

        IHost host = hostBuilder.Build();
        await host.RunAsync();
    }
}
