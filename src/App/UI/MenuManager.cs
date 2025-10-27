namespace App.UI;

public class MenuManager
{
    private readonly List<Guid> _walletIds;
    private readonly List<Guid> _transactionIds;

    public MenuManager(List<Guid> walletIds, List<Guid> transactionIds)
    {
        _walletIds = walletIds;
        _transactionIds = transactionIds;
    }

    public void DisplayMenu()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║               ТЕСТЕР API ЭНДПОИНТОВ                        ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("┌─ ЭНДПОИНТЫ КОШЕЛЬКОВ ─────────────────────────────────────┐");
        Console.ResetColor();
        Console.WriteLine("│  1  - Создать кошелёк                                     │");
        Console.WriteLine("│  2  - Получить все кошельки (с пагинацией)                │");
        Console.WriteLine("│  3  - Получить кошелёк по ID                              │");
        Console.WriteLine("│  4  - Переименовать кошелёк                               │");
        Console.WriteLine("│  5  - Удалить кошелёк                                     │");
        Console.WriteLine("│  6  - Получить топ расходов по кошелькам                  │");
        Console.WriteLine("└───────────────────────────────────────────────────────────┘");
        Console.WriteLine();
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("┌─ ЭНДПОИНТЫ ТРАНЗАКЦИЙ ────────────────────────────────────┐");
        Console.ResetColor();
        Console.WriteLine("│  7  - Добавить транзакцию в кошелёк                       │");
        Console.WriteLine("│  8  - Получить транзакции кошелька                        │");
        Console.WriteLine("│  9  - Получить транзакцию по ID                           │");
        Console.WriteLine("│  10 - Получить транзакции сгруппированные по типу         │");
        Console.WriteLine("└───────────────────────────────────────────────────────────┘");
        Console.WriteLine();
        
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("  0  - Выход");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("════════════════════════════════════════════════════════════");

        DisplayStoredIds();
    }

    private void DisplayStoredIds()
    {
        if (_walletIds.Any())
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"\n📝 Сохранённые ID кошельков: {string.Join(", ", _walletIds.Take(3))}");
            if (_walletIds.Count > 3)
                Console.WriteLine($"   ... и ещё {_walletIds.Count - 3}");
        }

        if (_transactionIds.Any())
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"📝 Сохранённые ID транзакций: {string.Join(", ", _transactionIds.Take(3))}");
            if (_transactionIds.Count > 3)
                Console.WriteLine($"   ... и ещё {_transactionIds.Count - 3}");
        }

        Console.ResetColor();
    }

    public void WaitForKeyPress()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\nНажмите любую клавишу для продолжения...");
        Console.ResetColor();
        Console.ReadKey();
    }
}
