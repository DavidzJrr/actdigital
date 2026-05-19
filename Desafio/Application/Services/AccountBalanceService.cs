using System;
using System.Threading;
using System.Threading.Tasks;
using Desafio.Application.Requests;
using Desafio.Application.Results;
using Desafio.Domain.Entities;
using Desafio.Domain.Repositories;
using Desafio.Domain.ValueObjects;

namespace Desafio.Application.Services;

public sealed class AccountBalanceService
{
    private readonly IAccountRepository _accountRepository;

    public AccountBalanceService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
    }

    public async Task DepositAsync(DepositRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var account = await _accountRepository.GetByCpfAsync(new Cpf(request.Cpf), cancellationToken);
        if (account is null) throw new InvalidOperationException("Account not found.");

        account.Deposit(new Money(request.Amount));
        await _accountRepository.SaveAsync(account, cancellationToken);
    }

    public async Task<TransferResult> TransferAsync(TransferRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var account = await _accountRepository.GetByCpfAsync(new Cpf(request.Cpf), cancellationToken);
        if (account is null) throw new InvalidOperationException("Account not found.");

        var amount = new Money(request.Amount);
        var requestedAt = request.Date;

        var availableDailyLimit = account.GetAvailableDailyLimit(requestedAt);

        if (!account.CanExecuteTransfer(amount, requestedAt))
        {
            return new TransferResult(false, "Insufficient balance or amount exceeds available PIX limit for today.", amount, account.PixLimit, account.Balance, availableDailyLimit);
        }

        var actor = new LimitConsumptionEvent();
        account.ConsumeDailyLimit(amount, actor, requestedAt);
        await _accountRepository.SaveAsync(account, cancellationToken);

        return new TransferResult(true, "Transfer allowed and executed.", amount, account.PixLimit, account.Balance, account.GetAvailableDailyLimit(requestedAt));
    }
}
