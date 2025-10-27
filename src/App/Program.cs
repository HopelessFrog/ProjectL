using App.Models;
using App.Services;
using App.UI;
using Microsoft.Extensions.Configuration;

// Загрузка конфигурации
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var apiSettings = configuration.GetSection("ApiSettings").Get<ApiSettings>() ?? new ApiSettings();

// Инициализация сервисов
var apiClient = new ApiClient(apiSettings.BaseUrl);
var walletIds = new List<Guid>();
var transactionIds = new List<Guid>();

var walletEndpoints = new WalletEndpoints(apiClient, walletIds);
var transactionEndpoints = new TransactionEndpoints(apiClient, walletIds, transactionIds);
var menuManager = new MenuManager(walletIds, transactionIds);

Console.OutputEncoding = System.Text.Encoding.UTF8;

while (true)
{
    menuManager.DisplayMenu();
    
    Console.Write("\nВведите номер: ");
    var choice = Console.ReadLine();

    try
    {
        switch (choice)
        {
            case "1":
                await walletEndpoints.CreateWallet();
                break;
            case "2":
                await walletEndpoints.GetAllWallets();
                break;
            case "3":
                await walletEndpoints.GetWalletById();
                break;
            case "4":
                await walletEndpoints.RenameWallet();
                break;
            case "5":
                await walletEndpoints.DeleteWallet();
                break;
            case "6":
                await walletEndpoints.GetTopExpenses();
                break;
            case "7":
                await transactionEndpoints.AddTransaction();
                break;
            case "8":
                await transactionEndpoints.GetTransactionsByWallet();
                break;
            case "9":
                await transactionEndpoints.GetTransactionById();
                break;
            case "10":
                await transactionEndpoints.GetTransactionsGroupedByType();
                break;
            case "0":
                ConsoleHelper.PrintSuccess("До свидания!");
                return;
            default:
                ConsoleHelper.PrintError("Неверный выбор!");
                break;
        }
    }
    catch (HttpRequestException ex)
    {
        ConsoleHelper.PrintError($"Ошибка подключения к API: {ex.Message}");
        ConsoleHelper.PrintError($"Убедитесь, что API запущен на {apiSettings.BaseUrl}");
    }
    catch (Exception ex)
    {
        ConsoleHelper.PrintError($"Ошибка: {ex.Message}");
    }

    menuManager.WaitForKeyPress();
}
