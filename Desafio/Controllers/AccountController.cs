using Desafio.Application.Requests;
using Desafio.Application.Results;
using Desafio.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Desafio.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AccountController : ControllerBase
{
    private readonly IPixLimitService _pixLimitService;
    private readonly IAccountBalanceService _accountBalanceService;

    public AccountController(IPixLimitService pixLimitService, IAccountBalanceService accountBalanceService)
    {
        _pixLimitService = pixLimitService;
        _accountBalanceService = accountBalanceService;
    }

    [HttpPost("balance")]
    public async Task<ActionResult<AccountBalanceResult>> GetBalanceAsync([FromBody] AccountLookupRequest request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Cpf))
        {
            return BadRequest("CPF is required to consult balance.");
        }

        var balance = await _accountBalanceService.GetBalanceAsync(request.Cpf);
        if (balance is null)
        {
            return NotFound("Account not found.");
        }

        return Ok(new AccountBalanceResult(balance.Amount));
    }

    [HttpPost("available-daily-limit")]
    public async Task<ActionResult<AvailableDailyLimitResult>> GetAvailableDailyLimitAsync([FromBody] AccountLookupRequest request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Cpf))
        {
            return BadRequest("CPF is required to consult available daily limit.");
        }

        var availableDailyLimit = await _accountBalanceService.GetAvailableDailyLimitAsync(request.Cpf);
        if (availableDailyLimit is null)
        {
            return NotFound("Account not found.");
        }

        return Ok(new AvailableDailyLimitResult(availableDailyLimit.Amount));
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterAccountRequest request)
    {
        await _pixLimitService.RegisterAccountAsync(request);
        return Created(string.Empty, null);
    }

    [HttpPost("validate-transaction")]
    public async Task<ActionResult<PixLimitResult>> ValidateTransactionAsync([FromBody] PixLimitRequest request)
    {
        var result = await _pixLimitService.ValidateTransactionAsync(request);
        return Ok(result);
    }

    [HttpPost("set-limit")]
    public async Task<IActionResult> SetLimitAsync([FromBody] UpdateAccountLimitRequest request)
    {
        await _pixLimitService.UpdateAccountLimitAsync(request);
        return Ok();
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> DepositAsync([FromBody] DepositRequest request)
    {
        await _accountBalanceService.DepositAsync(request);
        return NoContent();
    }

    [HttpPost("transfer")]
    public async Task<ActionResult<TransferResult>> TransferAsync([FromBody] TransferRequest request)
    {
        var result = await _accountBalanceService.TransferAsync(request);
        if (result.Allowed) return Ok(result);
        return BadRequest(result);
    }
}
