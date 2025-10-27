namespace Application.Wallets.Dtos;

public record WalletReadDto(Guid Id, string Name, string Currency, decimal InitialBalance, decimal CurrentBalance);