namespace BitsmapWorkerService.Domain.Entities;

public sealed record Bid(
    string usdValue,
    string btcValue
);