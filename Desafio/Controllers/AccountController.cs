using Desafio.Application.Requests;
using Desafio.Application.Results;
using Desafio.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Desafio.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AccountController : ControllerBase
{
    private readonly PixLimitService _pixLimitService;
    private readonly AccountBalanceService _accountBalanceService;

    public AccountController(PixLimitService pixLimitService, AccountBalanceService accountBalanceService)
    {
        _pixLimitService = pixLimitService;
        _accountBalanceService = accountBalanceService;
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
