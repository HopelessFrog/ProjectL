using Api.Requests.Wallets;
using Contrsacts.Wallets;
using FluentValidation;

namespace API.Wallets.Validators;

public class RenameWalletValidator : AbstractValidator<RenameWalletRequest>
{
    public RenameWalletValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}