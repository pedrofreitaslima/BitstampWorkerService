using System.Net.WebSockets;
using BitsmapWorkerService.Domain.Constants;
using BitsmapWorkerService.Services.Abstraction;

namespace BitsmapWorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ClientWebSocket _clientWebSocket;
    private readonly IConsumeStreaming _consumer;

    public Worker(ILogger<Worker> logger, ClientWebSocket clientWebSocket,
        IConsumeStreaming consumerStreaming)
    {
        _logger = logger;
        _clientWebSocket = clientWebSocket;
        _consumer = consumerStreaming;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ConnectServer(stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            
            await _consumer.RunAsync(
                _clientWebSocket,
                $"{SharedVariables.WebSocket.ChannelLiveOrderBook}{SharedVariables.WebSocket.CurrencyPairBtcUsd}",
                stoppingToken
            );
            await _consumer.RunAsync(
                _clientWebSocket,
                $"{SharedVariables.WebSocket.ChannelLiveOrderBook}{SharedVariables.WebSocket.CurrencyPairEthUsd}",
                stoppingToken
            );
        
            await Task.Delay(5000, stoppingToken);
        }
        await CloseServer(stoppingToken);
    }
    
    private async Task ConnectServer(CancellationToken stoppingToken)
    {
        await _clientWebSocket.ConnectAsync(new Uri(SharedVariables.WebSocket.ConnectionString), stoppingToken);
        _logger.LogInformation("Worker connected at server: {channelName}", SharedVariables.WebSocket.ConnectionString);
    }
    private async Task CloseServer(CancellationToken stoppingToken)
    {
        await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
            "Worker Service consume streaming done", stoppingToken);
        _logger.LogInformation("Worker closed connection at server: {channelName}", SharedVariables.WebSocket.ConnectionString);
    }
}