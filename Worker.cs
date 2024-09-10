using System.Net.WebSockets;
using System.Text;
using BitsmapWorkerService.Services.Abstraction;

namespace BitsmapWorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ClientWebSocket _client;
    private readonly IConsumeStreaming _orderBookBtcUsd;
    private readonly IConsumeStreaming _orderBookEthUsd;
    private const string ConnectionString = "wss://ws.bitstamp.net/";
    private const string ChannelNameBtcUsd = "order_book_btcusd";
    private const string ChannelNameEthUsd = "order_book_ethusd";

    public Worker(ILogger<Worker> logger, ClientWebSocket client,
        IConsumeStreaming orderBookBtcUsd, IConsumeStreaming orderBookEthUsd)
    {
        _logger = logger;
        _client = client;
        _orderBookBtcUsd = orderBookBtcUsd;
        _orderBookEthUsd = orderBookEthUsd;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _client.ConnectAsync(new Uri(ConnectionString), CancellationToken.None);
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            await _orderBookBtcUsd.RunAsync(_client, ChannelNameBtcUsd);
            await _orderBookEthUsd.RunAsync(_client, ChannelNameEthUsd);
            
            await Task.Delay(5000, stoppingToken);
        }
        
        await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
    }
}