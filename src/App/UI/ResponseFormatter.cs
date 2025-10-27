using System.Text.Json;

namespace App.UI;

public static class ResponseFormatter
{
    public static void PrintWallet(JsonElement wallet)
    {
        Console.WriteLine();
        Console.WriteLine("┌─────────────────────────── КОШЕЛЁК ────────────────────────────┐");
        
        PrintProperty("ID", wallet, "id", ConsoleColor.White);
        PrintProperty("Название", wallet, "name", ConsoleColor.Cyan);
        PrintProperty("Валюта", wallet, "currency", ConsoleColor.Yellow);
        PrintProperty("Начальный баланс", wallet, "initialBalance", ConsoleColor.Green, isMoney: true);
        PrintProperty("Текущий баланс", wallet, "currentBalance", ConsoleColor.Green, isMoney: true);
        
        Console.WriteLine("└────────────────────────────────────────────────────────────────┘");
    }

    public static void PrintWalletList(JsonElement response)
    {
        if (!response.TryGetProperty("items", out var items) || items.ValueKind != JsonValueKind.Array)
            return;

        var totalCount = response.TryGetProperty("totalCount", out var tc) ? tc.GetInt32() : 0;
        var page = response.TryGetProperty("page", out var p) ? p.GetInt32() : 1;
        var pageSize = response.TryGetProperty("pageSize", out var ps) ? ps.GetInt32() : 10;

        Console.WriteLine();
        Console.WriteLine($"┌─ СПИСОК КОШЕЛЬКОВ (страница {page}, всего: {totalCount}) ─────────────┐");
        Console.WriteLine("│                                                                  │");
        
        var index = 1;
        foreach (var wallet in items.EnumerateArray())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"│ {index}. ");
            Console.ResetColor();
            
            var name = GetProperty(wallet, "name");
            var currency = GetProperty(wallet, "currency");
            var id = GetProperty(wallet, "id");
            
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{name,-20} ");
            Console.ResetColor();
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"[{currency}]");
            Console.ResetColor();
            
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"  {id,-30} │");
            Console.ResetColor();
            
            index++;
        }
        
        Console.WriteLine("│                                                                  │");
        Console.WriteLine($"└─ Отображено {items.GetArrayLength()} из {totalCount} ─────────────────────────────────────┘");
    }

    public static void PrintTransaction(JsonElement transaction)
    {
        Console.WriteLine();
        Console.WriteLine("┌────────────────────────── ТРАНЗАКЦИЯ ──────────────────────────┐");
        
        PrintProperty("ID", transaction, "id", ConsoleColor.White);
        PrintProperty("Дата", transaction, "date", ConsoleColor.Cyan, isDate: true);
        PrintProperty("Сумма", transaction, "amount", ConsoleColor.Green, isMoney: true);
        
        if (transaction.TryGetProperty("type", out var typeElement))
        {
            var type = typeElement.GetInt32();
            Console.Write("│ ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{"Тип",-20}: ");
            Console.ResetColor();
            
            if (type == 0) // Income
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("⬆ Доход");
            }
            else // Expense
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("⬇ Расход");
            }
            Console.ResetColor();
            Console.WriteLine("".PadRight(36) + "│");
        }
        
        PrintProperty("Описание", transaction, "description", ConsoleColor.Yellow);
        PrintProperty("ID кошелька", transaction, "walletId", ConsoleColor.DarkGray);
        
        Console.WriteLine("└────────────────────────────────────────────────────────────────┘");
    }

    public static void PrintTransactionList(JsonElement response)
    {
        if (!response.TryGetProperty("items", out var items) || items.ValueKind != JsonValueKind.Array)
            return;

        var totalCount = response.TryGetProperty("totalCount", out var tc) ? tc.GetInt32() : 0;

        Console.WriteLine();
        Console.WriteLine($"┌─ СПИСОК ТРАНЗАКЦИЙ (всего: {totalCount}) ───────────────────────────┐");
        
        foreach (var transaction in items.EnumerateArray())
        {
            var date = GetProperty(transaction, "date");
            var amount = transaction.TryGetProperty("amount", out var a) ? a.GetDecimal() : 0;
            var type = transaction.TryGetProperty("type", out var t) ? t.GetInt32() : 1;
            var desc = GetProperty(transaction, "description");
            
            Console.Write("│ ");
            
            // Дата
            Console.ForegroundColor = ConsoleColor.DarkGray;
            if (DateTime.TryParse(date, out var dt))
                Console.Write($"{dt:dd.MM.yyyy HH:mm} ");
            else
                Console.Write($"{date,-16} ");
            Console.ResetColor();
            
            // Тип и сумма
            if (type == 0) // Income
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"⬆ +{amount,10:N2}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"⬇ -{amount,10:N2}");
            }
            Console.ResetColor();
            
            // Описание
            Console.ForegroundColor = ConsoleColor.Yellow;
            var description = desc.Length > 25 ? desc.Substring(0, 22) + "..." : desc;
            Console.WriteLine($"  {description,-25} │");
            Console.ResetColor();
        }
        
        Console.WriteLine("└──────────────────────────────────────────────────────────────────┘");
    }

    public static void PrintGroupedTransactions(JsonElement array)
    {
        if (array.ValueKind != JsonValueKind.Array)
            return;

        Console.WriteLine();
        Console.WriteLine("┌─ ТРАНЗАКЦИИ ПО ТИПАМ ─────────────────────────────────────────┐");
        
        foreach (var group in array.EnumerateArray())
        {
            var type = group.TryGetProperty("type", out var t) ? t.GetInt32() : 1;
            var total = group.TryGetProperty("totalAmount", out var ta) ? ta.GetDecimal() : 0;
            var transactions = group.TryGetProperty("transactions", out var trans) ? trans : default;
            var count = transactions.ValueKind == JsonValueKind.Array ? transactions.GetArrayLength() : 0;
            
            Console.Write("│ ");
            if (type == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("⬆ ДОХОДЫ:     ");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("⬇ РАСХОДЫ:    ");
            }
            Console.ResetColor();
            
            Console.ForegroundColor = type == 0 ? ConsoleColor.Green : ConsoleColor.Red;
            Console.Write($"{total,12:N2}");
            Console.ResetColor();
            
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"  ({count} транзакций)".PadRight(28) + "│");
            Console.ResetColor();
            
            if (transactions.ValueKind == JsonValueKind.Array && count > 0)
            {
                Console.WriteLine("│" + "".PadRight(64) + "│");
                var shown = 0;
                foreach (var tr in transactions.EnumerateArray())
                {
                    if (shown >= 3) break;
                    
                    var amount = tr.TryGetProperty("amount", out var amt) ? amt.GetDecimal() : 0;
                    var desc = GetProperty(tr, "description");
                    var description = desc.Length > 35 ? desc.Substring(0, 32) + "..." : desc;
                    
                    Console.Write("│   ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"{amount,10:N2}");
                    Console.ResetColor();
                    Console.Write("  ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"{description,-35}        │");
                    Console.ResetColor();
                    
                    shown++;
                }
                
                if (count > 3)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"│   ... и ещё {count - 3} транзакций".PadRight(66) + "│");
                    Console.ResetColor();
                }
            }
            
            Console.WriteLine("│" + "".PadRight(64) + "│");
        }
        
        Console.WriteLine("└────────────────────────────────────────────────────────────────┘");
    }

    private static void PrintProperty(string label, JsonElement element, string propertyName, 
        ConsoleColor valueColor, bool isMoney = false, bool isDate = false)
    {
        if (!element.TryGetProperty(propertyName, out var value))
            return;

        Console.Write("│ ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write($"{label,-20}: ");
        Console.ResetColor();
        
        Console.ForegroundColor = valueColor;
        
        if (isMoney && value.ValueKind == JsonValueKind.Number)
        {
            Console.Write($"{value.GetDecimal():N2}");
        }
        else if (isDate && value.ValueKind == JsonValueKind.String)
        {
            if (DateTime.TryParse(value.GetString(), out var dt))
                Console.Write($"{dt:dd.MM.yyyy HH:mm:ss}");
            else
                Console.Write(value.GetString());
        }
        else if (value.ValueKind == JsonValueKind.String)
        {
            Console.Write(value.GetString());
        }
        else
        {
            Console.Write(value.GetRawText());
        }
        
        Console.ResetColor();
        Console.WriteLine("".PadRight(36) + "│");
    }

    private static string GetProperty(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var value))
        {
            return value.ValueKind == JsonValueKind.String ? value.GetString() ?? "" : value.GetRawText();
        }
        return "";
    }
}
