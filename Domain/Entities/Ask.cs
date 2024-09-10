namespace BitsmapWorkerService.Domain.Entities;

public sealed record Ask(
    string usdValue,
    string ethValue
);