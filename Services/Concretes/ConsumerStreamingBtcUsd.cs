using System.Net.WebSockets;
using System.Text;
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
                Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, result.Count));
        }
    }
}