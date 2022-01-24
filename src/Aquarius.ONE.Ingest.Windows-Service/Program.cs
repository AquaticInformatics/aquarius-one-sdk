using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ONE;
using ONE.Ingest.WindowsService.Client;

using IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "Aquarius.ONE.Ingest";
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<ClientSDK>();
        services.AddHostedService<ClientService>();
        services.AddSingleton<ONE.Ingest.WindowsService.Agents.Test.AgentService>();
        services.AddSingleton<ONE.Ingest.WindowsService.Agents.CSV.AgentService>();
        services.AddSingleton<ONE.Ingest.WindowsService.Agents.OpenWeatherMap.AgentService>();
    })
    .Build();

await host.RunAsync();
