using App.UI;
using System.Text.Json;

namespace App.Services;

public class TransactionEndpoints
{
    private readonly ApiClient _apiClient;
    private readonly List<Guid> _walletIds;
    private readonly List<Guid> _transactionIds;

    public TransactionEndpoints(ApiClient apiClient, List<Guid> walletIds, List<Guid> transactionIds)
    {
        _apiClient = apiClient;
        _walletIds = walletIds;
        _transactionIds = transactionIds;
    }

    public async Task AddTransaction()
    {
        ConsoleHelper.PrintHeader("ДОБАВЛЕНИЕ ТРАНЗАКЦИИ");

        var walletId = ConsoleHelper.GetWalletId(_walletIds);
        if (walletId == Guid.Empty) return;

        var amount = ConsoleHelper.ReadDecimal("Введите сумму", 100m);
        
        Console.WriteLine("\nТип транзакции:");
        Console.WriteLine("  0 - Доход");
        Console.WriteLine("  1 - Расход");
        var type = ConsoleHelper.ReadInt("Введите тип", 1);

        var description = ConsoleHelper.ReadInput("Введите описание", "Тестовая транзакция");

        var request = new
        {
            amount,
            type,
            description
        };

        var response = await _apiClient.PostAsync($"/api/wallets/{walletId}/transactions", request);
        
        if (response.IsSuccess)
        {
            try
            {
                var transaction = JsonSerializer.Deserialize<JsonElement>(response.Content);
                ResponseFormatter.PrintTransaction(transaction);
                
                var transactionId = _apiClient.ExtractIdFromResponse(response.Content);
                if (transactionId.HasValue)
                {
                    _transactionIds.Add(transactionId.Value);
                    ConsoleHelper.PrintSuccess($"Транзакция создана с ID: {transactionId.Value}");
                }
            }
            catch
            {
                ConsoleHelper.PrintResponse(response);
            }
        }
        else
        {
            ConsoleHelper.PrintResponse(response);
        }
    }

    public async Task GetTransactionsByWallet()
    {
        ConsoleHelper.PrintHeader("ПОЛУЧЕНИЕ ТРАНЗАКЦИЙ КОШЕЛЬКА");

        var walletId = ConsoleHelper.GetWalletId(_walletIds);
        if (walletId == Guid.Empty) return;

        var page = ConsoleHelper.ReadInt("Введите номер страницы", 1);
        var pageSize = ConsoleHelper.ReadInt("Введите размер страницы", 10);

        var response = await _apiClient.GetAsync($"/api/wallets/{walletId}/transactions?page={page}&pageSize={pageSize}");
        
        if (response.IsSuccess)
        {
            try
            {
                var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
                ResponseFormatter.PrintTransactionList(json);
            }
            catch
            {
                ConsoleHelper.PrintResponse(response);
            }
        }
        else
        {
            ConsoleHelper.PrintResponse(response);
        }
    }

    public async Task GetTransactionById()
    {
        ConsoleHelper.PrintHeader("ПОЛУЧЕНИЕ ТРАНЗАКЦИИ ПО ID");

        Console.Write("Введите ID транзакции (Enter - использовать сохранённый): ");
        var input = Console.ReadLine();

        Guid transactionId;
        if (string.IsNullOrWhiteSpace(input) && _transactionIds.Any())
        {
            transactionId = _transactionIds.First();
            Console.WriteLine($"Используется сохранённый ID: {transactionId}");
        }
        else if (!Guid.TryParse(input, out transactionId))
        {
            ConsoleHelper.PrintError("Неверный ID!");
            return;
        }

        var response = await _apiClient.GetAsync($"/api/transactions/{transactionId}");
        
        if (response.IsSuccess)
        {
            try
            {
                var transaction = JsonSerializer.Deserialize<JsonElement>(response.Content);
                ResponseFormatter.PrintTransaction(transaction);
            }
            catch
            {
                ConsoleHelper.PrintResponse(response);
            }
        }
        else
        {
            ConsoleHelper.PrintResponse(response);
        }
    }

    public async Task GetTransactionsGroupedByType()
    {
        ConsoleHelper.PrintHeader("ТРАНЗАКЦИИ СГРУППИРОВАННЫЕ ПО ТИПУ");

        var year = ConsoleHelper.ReadInt("Введите год", 2025);
        var month = ConsoleHelper.ReadInt("Введите месяц (1-12)", 10);

        var response = await _apiClient.GetAsync($"/api/transactions/grouped-by-type?year={year}&month={month}");
        
        if (response.IsSuccess)
        {
            try
            {
                var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
                ResponseFormatter.PrintGroupedTransactions(json);
            }
            catch
            {
                ConsoleHelper.PrintResponse(response);
            }
        }
        else
        {
            ConsoleHelper.PrintResponse(response);
        }
    }
}
