namespace Application.Dtos;

public class PagedResult<T>
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public IReadOnlyList<T> Items { get; init; } = new List<T>();
}