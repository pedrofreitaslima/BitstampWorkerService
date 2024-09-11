namespace BitsmapWorkerService.Domain.Constants;

public static partial class SharedVariables
{
    public static class WebSocket
    {
        public static string ConnectionString { get; private set; } = "wss://ws.bitstamp.net";
        public static string ChannelLiveOrderBook { get; private set; } = "order_book_";
        public static string CurrencyPairBtcUsd { get; private set; } = "btcusd";
        public static string CurrencyPairEthUsd { get; private set; } = "ethusd";
        
    }
}