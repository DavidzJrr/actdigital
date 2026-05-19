using System;
using System.Threading;
using System.Threading.Tasks;
using Desafio.Application.Requests;
using Desafio.Application.Results;
using Desafio.Domain.Entities;
using Desafio.Domain.Repositories;
using Desafio.Domain.Services;
using Desafio.Domain.ValueObjects;

namespace Desafio.Application.Services;

public sealed class PixLimitService
{
    private readonly IAccountRepository _accountRepository;
    private readonly PixLimitEvaluator _pixLimitEvaluator;

    public PixLimitService(IAccountRepository accountRepository, PixLimitEvaluator pixLimitEvaluator)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        _pixLimitEvaluator = pixLimitEvaluator ?? throw new ArgumentNullException(nameof(pixLimitEvaluator));
    }

    public async Task<PixLimitResult> ValidateTransactionAsync(PixLimitRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var account = await _accountRepository.GetByCpfAsync(
            new Cpf(request.Cpf),
            cancellationToken);

        if (account is null)
        {
            throw new InvalidOperationException("Account not found.");
        }

        var evaluation = _pixLimitEvaluator.Evaluate(account, new Money(request.Amount), DateTimeOffset.UtcNow);
        return new PixLimitResult(evaluation);
    }

    public async Task RegisterAccountAsync(RegisterAccountRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var account = new Account(
            new Cpf(request.Cpf),
            new AgencyNumber(request.Agency),
            new AccountNumber(request.Account),
            Money.Default);

        await _accountRepository.SaveAsync(account, cancellationToken);
    }

    public async Task UpdateAccountLimitAsync(UpdateAccountLimitRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var account = await _accountRepository.GetByCpfAsync(
            new Cpf(request.Cpf),
            cancellationToken);

        if (account is null)
        {
            throw new InvalidOperationException("Account not found.");
        }

        var analyst = new FraudAnalyst(request.ActorId, request.ActorName, request.ActorRole);
        account.UpdatePixLimit(new Money(request.PixLimit), analyst);
        await _accountRepository.SaveAsync(account, cancellationToken);
    }

    private static FraudAnalyst? BuildAnalystIfProvided(string id, string name, string role)
    {
        if (string.IsNullOrWhiteSpace(id) && string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(role))
        {
            return null;
        }

        return new FraudAnalyst(id, name, role);
    }
}
