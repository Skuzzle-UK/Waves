using Microsoft.Extensions.Hosting;
using RazorConsole.Core;

namespace Waves;

internal class Program
{
    static async Task Main(string[] args)
    {
        IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args)
            .UseRazorConsole<Sample>();

        IHost host = hostBuilder.Build();
        await host.RunAsync();
    }
}
