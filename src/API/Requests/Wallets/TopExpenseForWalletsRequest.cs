namespace Contrsacts.Wallets;

public class TopExpenseForWalletsRequest : IYearMonthRequest
{
    public int Count { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
}