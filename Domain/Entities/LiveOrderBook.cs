using System.Text.Json.Serialization;

namespace BitsmapWorkerService.Domain.Entities;

public sealed class LiveOrderBook
{   
    [JsonPropertyName("bids")]
    public IEnumerable<string[]> Bids { get; set; } //List of top 100 bids.
    [JsonPropertyName("asks")]
    public IEnumerable<string[]> Asks { get; set; }//List of top 100 asks.
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } //Order book timestamp.
    [JsonPropertyName("microtimestamp")]
    public string Microtimestamp { get; set; } //Order book microtimestamp
}
	