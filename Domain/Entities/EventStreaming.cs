using System.Text.Json.Serialization;

namespace BitsmapWorkerService.Domain.Entities;

public sealed class EventStreaming
{
    [JsonPropertyName("data")]
    public LiveOrderBook LiveOrderBook { get; set; }
    [JsonPropertyName("channel")]
    public string Channel { get; set; }
    [JsonPropertyName("event")]
    public string Event { get; set; }
}