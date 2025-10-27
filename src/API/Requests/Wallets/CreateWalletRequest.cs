namespace Api.Requests.Wallets;

public class CreateWalletRequest
{
    public string? Name { get; set; }
    public string? Currency { get; set;}
    public decimal? InitialBalance { get;set; }
}