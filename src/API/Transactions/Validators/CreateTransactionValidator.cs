using Contrsacts.Transactions;
using Domain;
using FluentValidation;

namespace API.Transactions.Validators;

public class CreateTransactionValidator : AbstractValidator<CreateTransactionRequest>
{
    public CreateTransactionValidator()
    {
        RuleFor(x => x.Amount)
            .NotNull();
        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(x => Enum.IsDefined(typeof(TransactionType), x));
    }
}