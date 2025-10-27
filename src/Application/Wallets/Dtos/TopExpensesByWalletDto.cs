using Application.Transactions.Dtos;

namespace Application.Wallets.Dtos;

public class TopExpensesByWalletDto
{
    public Guid WalletId { get; set; }
    public string WalletName { get; set; } = string.Empty;
    public List<ExpenseTransactionDto> TopExpenses { get; set; } = new();
}