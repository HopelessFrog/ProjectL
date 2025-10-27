namespace Contrsacts.Transactions;

public class TransactionsGroupByTypeRequest : IYearMonthRequest
{
    public int Year { get; set; }
    public int Month { get; set; }
}