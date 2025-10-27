namespace API;

public static class Routes
{
    public const string Prefix = "api";

    public static class Endpoints
    {
        public const string Wallets = Prefix + "/wallets";

        public const string Wallet = Wallets + "/{walletId}";

        public const string TopExpenses = Wallets + "/top-expenses";

        public const string WalletTransactions = Wallet + "/transactions";

        public const string Transactions = Prefix + "/transactions";

        public const string Transaction = Transactions + "/{transactionId}";
        
        public const string GroupedByTypeTransactions = Transactions + "/grouped-by-type";

        public static string ForWallet(Guid id)
        {
            return ReplaceUrlSegment(Wallet, "walletId", id.ToString());
        }

        public static string ForTransaction(Guid id)
        {
            return ReplaceUrlSegment(Transaction, "transactionId", id.ToString());
        }

        private static string ReplaceUrlSegment(string template, string name, string value)
        {
            var escapedUri = Uri.EscapeDataString(value);
            return template.Replace('{' + name + '}', escapedUri);
        }
    }
}