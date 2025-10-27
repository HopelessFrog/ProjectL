using Contrsacts;
using FluentValidation;

namespace API.Validators;

public class YearMonthValidator<T> : AbstractValidator<T> where T : IYearMonthRequest
{
    public YearMonthValidator()
    {
        RuleFor(x => x.Year)
            .InclusiveBetween(1970, DateTime.Now.Year);

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12);
    }
}