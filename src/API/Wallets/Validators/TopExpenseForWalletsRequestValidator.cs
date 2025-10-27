using API.Validators;
using Contrsacts.Wallets;
using FluentValidation;

namespace API.Wallets.Validators;

public class TopExpenseForWalletsRequestValidator : YearMonthValidator<TopExpenseForWalletsRequest>
{
    public TopExpenseForWalletsRequestValidator()
    {
        RuleFor(x => x.Count)
            .GreaterThan(0);
    }
}