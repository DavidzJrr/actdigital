using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Desafio.Application.Requests;
using Desafio.Application.Services;
using Desafio.Domain.Entities;
using Desafio.Domain.Repositories;
using Desafio.Domain.ValueObjects;
using Xunit;

namespace Desafio.Tests.Application.Services;

public class AccountBalanceServiceTests
{
    [Fact]
    public async Task DepositAsync_AddsBalanceToExistingAccount()
    {
        var repo = new InMemoryAccountRepository();
        var service = new AccountBalanceService(repo);

        var account = new Account(
            new Cpf("32165498710"),
            new AgencyNumber("1234"),
            new AccountNumber("1234567"),
            "David Test",
            new Money(5000m),
            null,
            Guid.NewGuid(),
            new Money(1000m));

        await repo.SaveAsync(account);

        var request = new DepositRequest
        {
            Cpf = "321654987-10",
            Amount = 2500m,
            ActorId = "marisa-analyst-01",
            ActorName = "Marisa",
            ActorRole = "Analista de Fraude"
        };

        await service.DepositAsync(request);

        Assert.NotNull(repo.LastSavedAccount);
        Assert.Equal(3500m, repo.LastSavedAccount!.Balance.Amount);
    }

    [Fact]
    public async Task TransferAsync_ConsumesDailyLimitAndRecordsLimitConsumptionEvent()
    {
        var repo = new InMemoryAccountRepository();
        var service = new AccountBalanceService(repo);

        var account = new Account(
            new Cpf("32165498710"),
            new AgencyNumber("1234"),
            new AccountNumber("1234567"),
            new Money(5000m),
            null,
            Guid.NewGuid(),
            new Money(10000m));

        await repo.SaveAsync(account);

        var request = new TransferRequest
        {
            Cpf = "321654987-10",
            Amount = 2000m,
            PixKey = "key",
            Date = new DateTimeOffset(2026, 5, 19, 10, 0, 0, TimeSpan.Zero)
        };

        var result = await service.TransferAsync(request);

        Assert.True(result.Allowed);
        Assert.Equal(8000m, repo.LastSavedAccount!.Balance.Amount);
        Assert.Equal(2000m, repo.LastSavedAccount.DailyLimitConsumed.Amount);
        Assert.Equal(new DateOnly(2026, 5, 19), repo.LastSavedAccount.DailyLimitConsumedAt);
        Assert.Equal(3000m, result.AvailableDailyLimit.Amount);
        Assert.NotNull(repo.LastSavedAccount.LimitAudit);
        Assert.Equal("Consumo do Limite", repo.LastSavedAccount.LimitAudit!.Actor.Name);
        Assert.Equal("SystemEvent", repo.LastSavedAccount.LimitAudit.Actor.Role);
    }

    [Fact]
    public async Task TransferAsync_ResetsDailyLimitOnNextDay()
    {
        var repo = new InMemoryAccountRepository();
        var service = new AccountBalanceService(repo);

        var account = new Account(
            new Cpf("32165498710"),
            new AgencyNumber("1234"),
            new AccountNumber("1234567"),
            new Money(5000m),
            null,
            Guid.NewGuid(),
            new Money(10000m));

        await repo.SaveAsync(account);

        var firstRequest = new TransferRequest
        {
            Cpf = "321654987-10",
            Amount = 2000m,
            PixKey = "key",
            Date = new DateTimeOffset(2026, 5, 19, 10, 0, 0, TimeSpan.Zero)
        };

        var firstResult = await service.TransferAsync(firstRequest);
        Assert.True(firstResult.Allowed);

        var secondRequest = new TransferRequest
        {
            Cpf = "321654987-10",
            Amount = 5000m,
            PixKey = "key",
            Date = new DateTimeOffset(2026, 5, 20, 11, 0, 0, TimeSpan.Zero)
        };

        var secondResult = await service.TransferAsync(secondRequest);

        Assert.True(secondResult.Allowed);
        Assert.Equal(3000m, repo.LastSavedAccount!.Balance.Amount);
        Assert.Equal(5000m, repo.LastSavedAccount.DailyLimitConsumed.Amount);
        Assert.Equal(new DateOnly(2026, 5, 20), repo.LastSavedAccount.DailyLimitConsumedAt);
    }

    private sealed class InMemoryAccountRepository : IAccountRepository
    {
        private readonly Dictionary<string, Account> _store = new(StringComparer.OrdinalIgnoreCase);
        public Account? LastSavedAccount { get; private set; }

        public Task<Account?> GetByCpfAsync(Cpf cpf, CancellationToken cancellationToken = default)
        {
            _store.TryGetValue(cpf.Value, out var a);
            return Task.FromResult(a);
        }

        public Task SaveAsync(Account account, CancellationToken cancellationToken = default)
        {
            _store[account.Cpf.Value] = account;
            LastSavedAccount = account;
            return Task.CompletedTask;
        }

        public Task SaveAsync(Account account)
        {
            _store[account.Cpf.Value] = account;
            LastSavedAccount = account;
            return Task.CompletedTask;
        }
    }
}
