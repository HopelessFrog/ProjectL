using Domain;

namespace Application.Transactions.Dtos;

public class TransactionGroupByTypeDto
{
    public TransactionType Type { get; set; }
    public decimal TotalAmount { get; set; }
    public List<TransactionDto> Transactions { get; set; } = new();
}