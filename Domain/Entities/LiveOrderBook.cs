namespace BitsmapWorkerService.Domain.Entities;

public sealed record LiveOrderBook(
    Bid[] bids, //List of top 100 bids.
    Ask[] asks, //List of top 100 asks.
    string timestamp, //Order book timestamp.
    string microtimestamp //Order book microtimestamp
);
	