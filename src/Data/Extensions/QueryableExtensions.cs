using Application.Dtos;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Data.Extensions;

public static class QueryableExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<T>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items
        };
    }
    
    public static IQueryable<decimal> SelectSignedAmount(this IQueryable<Transaction> query)
    {
        return query.Select(t =>
            t.Type == TransactionType.Income ? t.Amount.Value : -t.Amount.Value);
    }
}