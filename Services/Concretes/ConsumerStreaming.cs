using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using BitsmapWorkerService.Domain.Constants;
using BitsmapWorkerService.Domain.Entities;
using BitsmapWorkerService.Services.Abstraction;

namespace BitsmapWorkerService.Services.Concretes;

public class ConsumerStreaming : IConsumeStreaming
{
    private readonly ILogger<ConsumerStreaming> _logger;
    
    public ConsumerStreaming(ILogger<ConsumerStreaming> logger)
    {
        _logger = logger;
    }
    public async Task RunAsync(ClientWebSocket webSocket ,string channelName, CancellationToken stoppingToken)
    {
        await SubscribeChannel(webSocket, channelName, stoppingToken);
        await ConsumeStreaming(webSocket);
        await UnsubscribeChannel(webSocket, channelName, stoppingToken);
    }
    private async Task SubscribeChannel(ClientWebSocket webSocket,string channelName, CancellationToken stoppingToken)
    {
        var data = Encoding.ASCII.GetBytes(
            "{\"event\": \"bts:subscribe\",\"data\":{\"channel\": \""+channelName+"\"}}");
        await webSocket.SendAsync(
            data,
            WebSocketMessageType.Text,
            true, 
            stoppingToken);
        _logger.LogInformation("Worker subscribed at: {channelName}", channelName);
    }
    private async Task UnsubscribeChannel(ClientWebSocket webSocket ,string channelName, CancellationToken stoppingToken)
    {
        var data = Encoding.ASCII.GetBytes(
            "{\"event\": \"bts:unsubscribe\",\"data\":{\"channel\": \""+channelName+"\"}}");
        await webSocket.SendAsync(
            data,
            WebSocketMessageType.Text,
            true, 
            stoppingToken);
        _logger.LogInformation("Worker unsubscribed at: {channelName}", channelName);
    }
    private async Task ConsumeStreaming(ClientWebSocket webSocket)
    {
        _logger.LogInformation("Worker is consuming the live order book streaming");
        bool consumeStreaming = true;
        string data = string.Empty;
        var buffer = new byte[1024];
        do
        {
            var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

            if (result.EndOfMessage && result.Count == 0)
                consumeStreaming = false;
            else
            {
                string dataStream = Encoding.ASCII.GetString(buffer, 0, result.Count);
                if (!dataStream.Contains("bts:subscription_succeeded"))
                    data += dataStream;
            }
        }
        while (consumeStreaming);

        EventStreaming eventStreaming = JsonSerializer.Deserialize<EventStreaming>(data) ??
                                      throw new ArgumentNullException("Streaming data is null");
        DisplayDataAnalytics(eventStreaming.LiveOrderBook.Bids.ToList(), eventStreaming.LiveOrderBook.Asks.ToList());
        await Persist(eventStreaming.LiveOrderBook);
        _logger.LogInformation("Worker consumed the live order book streaming");
    }
    private async Task Persist(LiveOrderBook liveOrderBook)
    {
        await Task.CompletedTask;
    }
    private void DisplayDataAnalytics(List<string[]> bids, List<string[]> asks)
    {
        var bidsUsdValues= bids.Select(bid => decimal.Parse(bid[0])).ToArray();
        var asksUsdValues= asks.Select(ask => decimal.Parse(ask[0])).ToArray();
        var highestBid = bidsUsdValues.Max();
        var highestAsk = asksUsdValues.Max();
        var lowestBid = bidsUsdValues.Min();
        var lowestAsk = asksUsdValues.Min();
        var averageBid = bidsUsdValues.Average();
        var averageAsk = asksUsdValues.Average();

        Console.WriteLine($"The highest price of the bid now is: {highestBid:C}");
        Console.WriteLine($"The highest price of the ask now is: {highestAsk:C}");
        Console.WriteLine($"The lowest price of the bid now is: {lowestBid:C}");
        Console.WriteLine($"The lowest price of the ask now is: {lowestAsk:C}");
        Console.WriteLine($"The average price of the bid now is: {averageBid:C}");
        Console.WriteLine($"The average price of the ask now is: {averageAsk:C}");
    }
}