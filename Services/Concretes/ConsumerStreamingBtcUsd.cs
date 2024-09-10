using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using BitsmapWorkerService.Domain.Entities;
using BitsmapWorkerService.Services.Abstraction;

namespace BitsmapWorkerService.Services.Concretes;

public class ConsumerStreamingBtcUsd : IConsumeStreaming
{
    private readonly ILogger<Worker> _logger;
    
    public ConsumerStreamingBtcUsd(ILogger<Worker> logger)
    {
        _logger = logger;
    }
    public async Task RunAsync(ClientWebSocket webSocket ,string channelName)
    {
        _logger.LogInformation("Worker subscribe in: {channelName}", channelName);
        var data = Encoding.ASCII.GetBytes(
            "{\"event\": \"bts:subscribe\",\"data\":{\"channel\": \""+channelName+"\"}}");
        await webSocket.SendAsync(
            data,
            WebSocketMessageType.Text, // Type
            true, //EndOfMessage
            CancellationToken.None);
        
        await ConsumeStreaming(webSocket);
    }
    
    private async Task ConsumeStreaming(ClientWebSocket webSocket)
    {
        var buffer = new byte[256];
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
            else
            {
                string dataStream = Encoding.ASCII.GetString(buffer, 0, result.Count);
                LiveOrderBook liveOrderBook = JsonSerializer.Deserialize<LiveOrderBook>(dataStream) ?? throw new InvalidOperationException();
                DisplayDataAnalytics(liveOrderBook.bids, liveOrderBook.asks);
                await PersistData(liveOrderBook);
            }
        }
    }
    
    private void DisplayDataAnalytics(Bid[] bids, Ask[] asks)
    {
        ShowHighestPrice(bids, asks);
        ShowLowestPrice(bids, asks);
        ShowLastAveragePrice(bids, asks);
        ShowAveragePriceNow(bids, asks);
    }
    
    private async Task PersistData(LiveOrderBook liveOrderBook)
    {
        await Task.CompletedTask;
    }

    private void ShowHighestPrice(Bid[] bids, Ask[] asks)
    {
        var highestPriceBid = bids
            .ToList()
            .AsReadOnly()
            .MaxBy(b => b.usdValue);
        
        var highestPriceAks = asks
            .ToList()
            .AsReadOnly()
            .MaxBy(b => b.usdValue);
        
        Console.WriteLine($"The highest price of the bid now is: {highestPriceBid?.usdValue}");
        Console.WriteLine($"The highest price of the ask now is: {highestPriceAks?.usdValue}");
    }
    
    private void ShowLowestPrice(Bid[] bids, Ask[] asks)
    {
        var lowestPriceBid = bids
            .ToList()
            .AsReadOnly()
            .MinBy(b => b.usdValue);
        
        var lowestPriceAsk = asks
            .ToList()
            .AsReadOnly()
            .MaxBy(b => b.usdValue);
        
        Console.WriteLine($"The highest price of the bid now is: {lowestPriceBid?.usdValue}");
        Console.WriteLine($"The highest price of the ask now is: {lowestPriceAsk?.usdValue}");
    }
    private void ShowLastAveragePrice(Bid[] bids, Ask[] asks)
    {
        var averagePriceBid = bids
            .ToList()
            .AsReadOnly()
            .Average(b =>  Convert.ToDecimal(b.usdValue));
        
        var averagePriceAsk = asks
            .ToList()
            .AsReadOnly()
            .Average(b => Convert.ToDecimal(b.usdValue));
        
        Console.WriteLine($"The last average price of the bid is: {averagePriceBid:C}");
        Console.WriteLine($"The last average price of the ask is: {averagePriceAsk:C}");
    }
    
    private void ShowAveragePriceNow(Bid[] bids, Ask[] asks)
    {
        var averagePriceBid = bids
            .ToList()
            .AsReadOnly()
            .Average(b =>  Convert.ToDecimal(b.usdValue));
        
        var averagePriceAsk = asks
            .ToList()
            .AsReadOnly()
            .Average(b => Convert.ToDecimal(b.usdValue));
        
        Console.WriteLine($"The average price of the bid now is: {averagePriceBid:C}");
        Console.WriteLine($"The average price of the ask now is: {averagePriceAsk:C}");
    }
}