using System.Net.WebSockets;

namespace BitsmapWorkerService.Services.Abstraction;

public interface IConsumeStreaming
{
    Task RunAsync(ClientWebSocket webSocket, string channelName,  CancellationToken stoppingToken);
}