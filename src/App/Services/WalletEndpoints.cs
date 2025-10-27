using App.UI;
using System.Text.Json;

namespace App.Services;

public class WalletEndpoints
{
    private readonly ApiClient _apiClient;
    private readonly List<Guid> _walletIds;

    public WalletEndpoints(ApiClient apiClient, List<Guid> walletIds)
    {
        _apiClient = apiClient;
        _walletIds = walletIds;
    }

    public async Task CreateWallet()
    {
        ConsoleHelper.PrintHeader("СОЗДАНИЕ КОШЕЛЬКА");

        var name = ConsoleHelper.ReadInput("Введите название кошелька", "Мой Кошелёк");
        var currency = ConsoleHelper.ReadInput("Введите валюту (USD/EUR/RUB)", "USD");
        var balance = ConsoleHelper.ReadDecimal("Введите начальный баланс", 1000m);

        var request = new
        {
            name,
            currency,
            initialBalance = balance
        };

        var response = await _apiClient.PostAsync("/api/wallets", request);
        
        if (response.IsSuccess)
        {
            try
            {
                var wallet = JsonSerializer.Deserialize<JsonElement>(response.Content);
                ResponseFormatter.PrintWallet(wallet);
                
                var walletId = _apiClient.ExtractIdFromResponse(response.Content);
                if (walletId.HasValue)
                {
                    _walletIds.Add(walletId.Value);
                    ConsoleHelper.PrintSuccess($"Кошелёк создан с ID: {walletId.Value}");
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

    public async Task GetAllWallets()
    {
        ConsoleHelper.PrintHeader("ПОЛУЧЕНИЕ ВСЕХ КОШЕЛЬКОВ");

        var page = ConsoleHelper.ReadInt("Введите номер страницы", 1);
        var pageSize = ConsoleHelper.ReadInt("Введите размер страницы", 10);

        var response = await _apiClient.GetAsync($"/api/wallets?page={page}&pageSize={pageSize}");
        
        if (response.IsSuccess)
        {
            try
            {
                var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
                ResponseFormatter.PrintWalletList(json);
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

    public async Task GetWalletById()
    {
        ConsoleHelper.PrintHeader("ПОЛУЧЕНИЕ КОШЕЛЬКА ПО ID");

        var walletId = ConsoleHelper.GetWalletId(_walletIds);
        if (walletId == Guid.Empty) return;

        var response = await _apiClient.GetAsync($"/api/wallets/{walletId}");
        
        if (response.IsSuccess)
        {
            try
            {
                var wallet = JsonSerializer.Deserialize<JsonElement>(response.Content);
                ResponseFormatter.PrintWallet(wallet);
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

    public async Task RenameWallet()
    {
        ConsoleHelper.PrintHeader("ПЕРЕИМЕНОВАНИЕ КОШЕЛЬКА");

        var walletId = ConsoleHelper.GetWalletId(_walletIds);
        if (walletId == Guid.Empty) return;

        var newName = ConsoleHelper.ReadInput("Введите новое название", null, required: true);
        if (string.IsNullOrWhiteSpace(newName))
        {
            ConsoleHelper.PrintError("Название не может быть пустым!");
            return;
        }

        var request = new { name = newName };
        var response = await _apiClient.PatchAsync($"/api/wallets?walletId={walletId}", request);

        if (string.IsNullOrEmpty(response.Content))
            ConsoleHelper.PrintSuccess("Кошелёк успешно переименован!");
        else
            ConsoleHelper.PrintResponse(response);
    }

    public async Task DeleteWallet()
    {
        ConsoleHelper.PrintHeader("УДАЛЕНИЕ КОШЕЛЬКА");

        var walletId = ConsoleHelper.GetWalletId(_walletIds);
        if (walletId == Guid.Empty) return;

        if (!ConsoleHelper.Confirm("Вы уверены, что хотите удалить кошелёк?"))
            return;

        var response = await _apiClient.DeleteAsync($"/api/wallets/{walletId}");

        if (string.IsNullOrEmpty(response.Content))
        {
            ConsoleHelper.PrintSuccess("Кошелёк успешно удалён!");
            _walletIds.Remove(walletId);
        }
        else
        {
            ConsoleHelper.PrintResponse(response);
        }
    }

    public async Task GetTopExpenses()
    {
        ConsoleHelper.PrintHeader("ТОП РАСХОДОВ ПО КОШЕЛЬКАМ");

        var year = ConsoleHelper.ReadInt("Введите год", 2025);
        var month = ConsoleHelper.ReadInt("Введите месяц (1-12)", 10);
        var count = ConsoleHelper.ReadInt("Введите количество", 5);

        var response = await _apiClient.GetAsync($"/api/wallets/top-expenses?year={year}&month={month}&count={count}");
        ConsoleHelper.PrintResponse(response);
    }
}
