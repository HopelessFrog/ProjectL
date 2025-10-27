using Application.Dtos;
using Application.Transactions;
using Application.Transactions.Dtos;
using Contrsacts;
using Contrsacts.Transactions;
using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Transactions;

[ApiController]
public sealed class TransactionController : ControllerBase
{
    private readonly IAdvancedTransactionsQueryService _advancedTransactionsQueryService;
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService,
        IAdvancedTransactionsQueryService advancedTransactionsQueryService)
    {
        _transactionService = transactionService;
        _advancedTransactionsQueryService = advancedTransactionsQueryService;
    }

    [HttpGet(Routes.Endpoints.Transactions)]
    [ProducesResponseType<TransactionDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<PagedResult<TransactionDto>> GetTransactions([FromRoute] Guid walletId,
        [FromQuery] PagedRequest pagedRequest, CancellationToken ct = default)
    {
        return await _transactionService.GetTransactionsByWalletId(walletId, pagedRequest.Page, pagedRequest.PageSize,
            ct);
    }

    [HttpGet(Routes.Endpoints.Transaction)]
    [ProducesResponseType<TransactionDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid transactionId, CancellationToken ct = default)
    {
        var result = await _transactionService.GetTransactionAsync(transactionId, ct);
        return  result.ToActionResult();
    }


    [HttpGet(Routes.Endpoints.GroupedByTypeTransactions)]
    [ProducesResponseType<TransactionGroupByTypeDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IReadOnlyCollection<TransactionGroupByTypeDto>> GetGrupedByTypeTransactions(
        [FromQuery] TransactionsGroupByTypeRequest request, CancellationToken ct = default)
    {
        return await _advancedTransactionsQueryService.GetTransactionsGroupedByTypeAsync(request.Year, request.Month,
            ct);
    }
}