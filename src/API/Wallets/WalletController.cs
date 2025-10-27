using Api.Requests.Wallets;
using Application.Dtos;
using Application.Transactions.Dtos;
using Application.Wallets;
using Application.Wallets.Dtos;
using Contrsacts;
using Contrsacts.Transactions;
using Contrsacts.Wallets;
using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Wallets;

[ApiController]
public sealed class WalletController : ControllerBase
{
    private readonly IAdvancedWalletsQueryService _advancedWalletsQueryService;
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService, IAdvancedWalletsQueryService advancedWalletsQueryService)
    {
        _walletService = walletService;
        _advancedWalletsQueryService = advancedWalletsQueryService;
    }

    [HttpGet(Routes.Endpoints.Wallets)]
    [ProducesResponseType<PagedResult<WalletPreviewDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<PagedResult<WalletPreviewDto>> GetWallets([FromQuery] PagedRequest pagedRequest,
        CancellationToken ct = default)
    {
        return await _walletService.GetWalletsAsync(pagedRequest.Page, pagedRequest.PageSize, ct);
    }

    [HttpGet(Routes.Endpoints.Wallet)]
    [ProducesResponseType<WalletReadDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetWallet([FromRoute] Guid walletId,
        CancellationToken ct = default)
    {
        return await _walletService.GetWalletByIdAsync(walletId, ct).ToActionResult();
    }

    [HttpPost(Routes.Endpoints.Wallets)]
    [ProducesResponseType<WalletReadDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateWalletRequest request)
    {
        var result =
            await _walletService.CreateWalletAsync(request.Name, request.Currency, request.InitialBalance.Value);

        if (result.IsFailed)
            return result.ToActionResult();

        var wallet = result.Value;

        return Created(Routes.Endpoints.ForWallet(result.Value.Id),
            new WalletReadDto(wallet.Id, wallet.Name.Value, wallet.Currency.Value, wallet.InitialBalance.Value,
                wallet.InitialBalance.Value));
    }

    [HttpPatch(Routes.Endpoints.Wallet)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Rename([FromRoute] Guid walletId, [FromBody] RenameWalletRequest request)
    {
        var result = await _walletService.RenameWalletAsync(walletId, request.Name);

        if (result.IsFailed) return result.ToActionResult();

        return NoContent();
    }

    [HttpDelete(Routes.Endpoints.Wallet)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid walletId)
    {
        var result = await _walletService.DeleteWalletAsync(walletId);

        if (result.IsFailed)
            return result.ToActionResult();

        return NoContent();
    }

    [HttpPost(Routes.Endpoints.WalletTransactions)]
    [ProducesResponseType<TransactionDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddTransaction([FromRoute] Guid walletId,
        [FromBody] CreateTransactionRequest request)
    {
        var result =
            await _walletService.AddTransaction(new CreateTransactionDto(
                request.Amount.Value,
                request.Type.Value,
                request.Description,
                walletId));

        if (result.IsFailed)
            return result.ToActionResult();

        var transaction = result.Value;

        var response = new TransactionDto(
            transaction.Id,
            transaction.Date,
            transaction.Amount.Value,
            transaction.Type,
            transaction.Description.Value,
            transaction.WalletId);

        return Created(Routes.Endpoints.ForTransaction(result.Value.Id), response);
    }

    [HttpGet(Routes.Endpoints.TopExpenses)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IReadOnlyCollection<TopExpensesByWalletDto>> GetTopExpenseForWallets(
        [FromBody] TopExpenseForWalletsRequest request)
    {
        return await _advancedWalletsQueryService.GetAllTopExpensesByWalletAsync(request.Year, request.Month,
            request.Count);
    }
}