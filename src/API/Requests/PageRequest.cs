namespace Contrsacts;

public class PagedRequest
{
    public const int MaxPageSize = 100;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}