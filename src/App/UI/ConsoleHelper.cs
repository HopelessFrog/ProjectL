using System.Text.Json;
using App.Services;

namespace App.UI;

public static class ConsoleHelper
{
    public static void PrintHeader(string title)
    {
        Console.WriteLine($"\n╔══════════════════════════════════════════════════════╗");
        Console.WriteLine($"║  {title.PadRight(50)}║");
        Console.WriteLine($"╚══════════════════════════════════════════════════════╝\n");
    }

    public static void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"✓ {message}");
        Console.ResetColor();
    }

    public static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"✗ {message}");
        Console.ResetColor();
    }

    public static void PrintResponse(ApiResponse response)
    {
        Console.WriteLine();
        Console.WriteLine("┌─────────────────────────────────────────────────────────┐");
        
        // Статус
        if (response.IsSuccess)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("│ ✓ Статус: ");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("│ ✗ Статус: ");
        }
        Console.WriteLine($"{(int)response.StatusCode} {response.StatusCode}".PadRight(46) + "│");
        Console.ResetColor();
        
        Console.WriteLine("├─────────────────────────────────────────────────────────┤");
        
        // Красивый вывод JSON
        if (!string.IsNullOrWhiteSpace(response.Content))
        {
            try
            {
                var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
                PrintJsonElement(json, 0);
            }
            catch
            {
                Console.WriteLine($"│ {response.Content.PadRight(55)}│");
            }
        }
        
        Console.WriteLine("└─────────────────────────────────────────────────────────┘");
    }
    
    private static void PrintJsonElement(JsonElement element, int indent, string propertyName = "")
    {
        var indentStr = new string(' ', indent * 2);
        var prefix = "│ " + indentStr;
        
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                if (!string.IsNullOrEmpty(propertyName))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"{prefix}{propertyName}:".PadRight(58) + "│");
                    Console.ResetColor();
                }
                
                foreach (var property in element.EnumerateObject())
                {
                    PrintJsonElement(property.Value, indent + 1, property.Name);
                }
                break;
                
            case JsonValueKind.Array:
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{prefix}{propertyName}: [{element.GetArrayLength()} элементов]".PadRight(58) + "│");
                Console.ResetColor();
                
                var index = 0;
                foreach (var item in element.EnumerateArray())
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"{prefix}  [{index}]:".PadRight(58) + "│");
                    Console.ResetColor();
                    PrintJsonElement(item, indent + 2, "");
                    index++;
                    if (index > 5) // Ограничение для больших массивов
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine($"{prefix}  ... и ещё {element.GetArrayLength() - 5} элементов".PadRight(58) + "│");
                        Console.ResetColor();
                        break;
                    }
                }
                break;
                
            case JsonValueKind.String:
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"{prefix}{propertyName}: ");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                var strValue = element.GetString() ?? "";
                if (strValue.Length > 40)
                    strValue = strValue.Substring(0, 37) + "...";
                Console.WriteLine(strValue.PadRight(58 - prefix.Length - propertyName.Length - 2) + "│");
                Console.ResetColor();
                break;
                
            case JsonValueKind.Number:
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"{prefix}{propertyName}: ");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(element.GetRawText().PadRight(58 - prefix.Length - propertyName.Length - 2) + "│");
                Console.ResetColor();
                break;
                
            case JsonValueKind.True:
            case JsonValueKind.False:
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"{prefix}{propertyName}: ");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(element.GetBoolean().ToString().PadRight(58 - prefix.Length - propertyName.Length - 2) + "│");
                Console.ResetColor();
                break;
                
            case JsonValueKind.Null:
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"{prefix}{propertyName}: ");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("null".PadRight(58 - prefix.Length - propertyName.Length - 2) + "│");
                Console.ResetColor();
                break;
        }
    }

    public static string ReadInput(string prompt, string? defaultValue = null, bool required = false)
    {
        if (defaultValue != null)
            Console.Write($"{prompt} (по умолчанию: {defaultValue}): ");
        else
            Console.Write($"{prompt}: ");

        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            if (required && defaultValue == null)
            {
                PrintError("Это поле обязательно!");
                return string.Empty;
            }
            return defaultValue ?? string.Empty;
        }

        return input;
    }

    public static int ReadInt(string prompt, int defaultValue)
    {
        Console.Write($"{prompt} (по умолчанию: {defaultValue}): ");
        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
            return defaultValue;

        return int.TryParse(input, out var result) ? result : defaultValue;
    }

    public static decimal ReadDecimal(string prompt, decimal defaultValue)
    {
        Console.Write($"{prompt} (по умолчанию: {defaultValue}): ");
        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
            return defaultValue;

        return decimal.TryParse(input, out var result) ? result : defaultValue;
    }

    public static bool Confirm(string message)
    {
        Console.Write($"{message} (д/н): ");
        var input = Console.ReadLine()?.ToLower();
        return input == "д" || input == "y" || input == "yes" || input == "да";
    }

    public static Guid GetWalletId(List<Guid> storedWalletIds)
    {
        Console.Write("Введите ID кошелька (Enter - использовать сохранённый): ");
        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input) && storedWalletIds.Any())
        {
            var walletId = storedWalletIds.First();
            Console.WriteLine($"Используется сохранённый ID: {walletId}");
            return walletId;
        }

        if (Guid.TryParse(input, out var parsedId))
            return parsedId;

        PrintError("Неверный ID!");
        return Guid.Empty;
    }

    private static string FormatJson(string json)
    {
        try
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
            return JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
        }
        catch
        {
            return json;
        }
    }
}
