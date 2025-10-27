using Domain;

namespace Contrsacts.Transactions;

public class CreateTransactionRequest
{
    public decimal? Amount { get; set; }
    public TransactionType? Type { get; set; }
    public string? Description { get; set; }
}