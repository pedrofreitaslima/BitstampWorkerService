using System.Net.WebSockets;
using BitsmapWorkerService;
using BitsmapWorkerService.Services.Abstraction;
using BitsmapWorkerService.Services.Concretes;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<IConsumeStreaming,ConsumerStreaming>();
        services.AddTransient<ClientWebSocket>();
    })
    .Build();

host.Run();