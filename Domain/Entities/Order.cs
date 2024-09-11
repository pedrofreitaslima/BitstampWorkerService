namespace BitsmapWorkerService.Domain.Entities;

public sealed class Order
{
    public Order()
    {
        
    }

    public Order(decimal usd, decimal crypto)
    {
        Usd = usd;
        Crypto = crypto;
    }
    public decimal Usd { get; set; }
    public decimal Crypto { get; set; }
}