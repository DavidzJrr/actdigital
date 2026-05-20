using System;
using System.Threading;
using System.Threading.Tasks;
using Desafio.Application.Events;
using Desafio.Application.Requests;
using Desafio.Application.Results;
using Desafio.Domain.Entities;
using Desafio.Domain.Events;
using Desafio.Domain.Repositories;
using Desafio.Domain.ValueObjects;

namespace Desafio.Application.Services;

public sealed class AccountBalanceService : IAccountBalanceService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public AccountBalanceService(IAccountRepository accountRepository, IDomainEventDispatcher eventDispatcher)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
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

        var actor = new SystemActor();
        account.ConsumeDailyLimit(amount, actor, requestedAt);
        await _accountRepository.SaveAsync(account, cancellationToken);

        var consumptionEvent = new DailyLimitConsumedEvent(
            account.AccountId,
            account.Cpf.Value,
            amount,
            account.Balance,
            account.GetAvailableDailyLimit(requestedAt),
            DateTimeOffset.UtcNow);

        await _eventDispatcher.DispatchAsync(consumptionEvent, cancellationToken);

        return new TransferResult(true, "Transfer allowed and executed.", amount, account.PixLimit, account.Balance, account.GetAvailableDailyLimit(requestedAt));
    }

    public async Task<Money?> GetBalanceAsync(string cpf, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cpf)) throw new ArgumentException("CPF is required.", nameof(cpf));

        var account = await _accountRepository.GetByCpfAsync(new Cpf(cpf), cancellationToken);
        return account?.Balance;
    }

    public async Task<Money?> GetAvailableDailyLimitAsync(string cpf, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cpf)) throw new ArgumentException("CPF is required.", nameof(cpf));

        var account = await _accountRepository.GetByCpfAsync(new Cpf(cpf), cancellationToken);
        if (account is null)
        {
            return null;
        }

        var availableLimit = account.GetAvailableDailyLimit(DateTimeOffset.UtcNow);
        var limitCheckedEvent = new PixLimitCheckedEvent(
            account.AccountId,
            account.Cpf.Value,
            account.PixLimit,
            availableLimit,
            DateTimeOffset.UtcNow);

        await _eventDispatcher.DispatchAsync(limitCheckedEvent, cancellationToken);
        return availableLimit;
    }
}
