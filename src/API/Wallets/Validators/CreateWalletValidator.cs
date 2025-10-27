using Api.Requests.Wallets;
using FluentValidation;

namespace API.Wallets.Validators;

public class CreateWalletValidator : AbstractValidator<CreateWalletRequest>
{
    public CreateWalletValidator()
    {
        RuleFor(r => r.Name).NotEmpty();
        RuleFor(r => r.Currency).NotEmpty();
        RuleFor(r => r.InitialBalance).NotNull();
    }
}