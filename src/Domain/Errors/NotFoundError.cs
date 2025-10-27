using FluentResults;

namespace Domain.Errors;

public class NotFoundError : Error
{
    public NotFoundError(string entityName)
        : base($"'{entityName}' не найден(а)")
    {
        EntityName = entityName;
    }

    public string EntityName { get; }
    public object Id { get; }
}