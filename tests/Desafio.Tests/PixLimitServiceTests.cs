using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Desafio.Application.Requests;
using Desafio.Application.Services;
using Desafio.Domain.Entities;
using Desafio.Domain.Repositories;
using Desafio.Domain.Services;
using Desafio.Domain.ValueObjects;
using Xunit;

namespace Desafio.Tests.Application.Services;

public class PixLimitServiceTests
{
    [Fact]
    public async Task RegisterAnalystAsync_SavesMarisaAsFraudAnalystProfile()
    {
        var analystRepository = new FakeAnalystRepository();
        var analystService = new AnalystService(analystRepository);

        var request = new RegisterAnalystRequest
        {
            Id = "marisa-analyst-01",
            Name = "Marisa",
            Role = "Analista de Fraude"
        };

        await analystService.RegisterAnalystAsync(request);

        Assert.NotNull(analystRepository.LastSavedAnalyst);
        Assert.Equal("marisa-analyst-01", analystRepository.LastSavedAnalyst!.Id);
        Assert.Equal("Marisa", analystRepository.LastSavedAnalyst.Name);
        Assert.Equal("Analista de Fraude", analystRepository.LastSavedAnalyst.Role);
    }

    [Fact]
    public async Task AnalystCreatesAccountForDavidAndThenUpdatesLimitTo10000()
    {
        var accountRepository = new InMemoryAccountRepository();
        var pixLimitEvaluator = new PixLimitEvaluator();
        var pixLimitService = new PixLimitService(accountRepository, pixLimitEvaluator);

        var registrationRequest = new RegisterAccountRequest
        {
            Cpf = "321654987-10",
            Agency = "1234",
            Account = "1234567"
        };

        await pixLimitService.RegisterAccountAsync(registrationRequest);

        var savedAccount = accountRepository.LastSavedAccount;
        Assert.NotNull(savedAccount);
        Assert.Equal("32165498710", savedAccount!.Cpf.Value);
        Assert.Equal("1234", savedAccount.Agency.Value);
        Assert.Equal("1234567", savedAccount.Number.Value);
        Assert.Equal("David Test", savedAccount.ClientName);
        Assert.Equal(5000m, savedAccount.PixLimit.Amount);
        Assert.True(savedAccount.PixLimit.Amount < 10000m);
        Assert.Null(savedAccount.LimitAudit);

        var updateRequest = new UpdateAccountLimitRequest
        {
            Cpf = "321654987-10",
            Agency = "1234",
            Account = "1234567",
            PixLimit = 10000m,
            ActorId = "marisa-analyst-01",
            ActorName = "Marisa",
            ActorRole = "Analista de Fraude"
        };

        await pixLimitService.UpdateAccountLimitAsync(updateRequest);

        var updatedAccount = accountRepository.LastSavedAccount;
        Assert.NotNull(updatedAccount);
        Assert.Equal(10000m, updatedAccount!.PixLimit.Amount);
        Assert.NotNull(updatedAccount.LimitAudit);
        Assert.Equal("Marisa", updatedAccount.LimitAudit!.Actor.Name);
        Assert.Equal("Analista de Fraude", updatedAccount.LimitAudit.Actor.Role);
        Assert.True(updatedAccount.LimitAudit.ChangedAt <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task AccountInitialLimitIsBelow10000BeforeUpdate()
    {
        var accountRepository = new InMemoryAccountRepository();
        var pixLimitEvaluator = new PixLimitEvaluator();
        var pixLimitService = new PixLimitService(accountRepository, pixLimitEvaluator);

        var registrationRequest = new RegisterAccountRequest
        {
            Cpf = "321654987-10",
            Agency = "1234",
            Account = "1234567"
        };

        await pixLimitService.RegisterAccountAsync(registrationRequest);

        var savedAccount = accountRepository.LastSavedAccount;
        Assert.NotNull(savedAccount);
        Assert.Equal(5000m, savedAccount!.PixLimit.Amount);
        Assert.True(savedAccount.PixLimit.Amount < 10000m);
        Assert.Equal("32165498710", savedAccount.Cpf.Value);
    }

    private sealed class FakeAnalystRepository : IAnalystRepository
    {
        public FraudAnalyst? LastSavedAnalyst { get; private set; }

        public Task<FraudAnalyst?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<FraudAnalyst?>(null);
        }

        public Task SaveAsync(FraudAnalyst analyst, CancellationToken cancellationToken = default)
        {
            LastSavedAnalyst = analyst;
            return Task.CompletedTask;
        }
    }

    private sealed class InMemoryAccountRepository : IAccountRepository
    {
        private readonly Dictionary<string, Account> _storage = new(StringComparer.OrdinalIgnoreCase);

        public Account? LastSavedAccount { get; private set; }

        public Task<Account?> GetByCpfAsync(Cpf cpf, CancellationToken cancellationToken = default)
        {
            _storage.TryGetValue(cpf.Value, out var account);
            return Task.FromResult(account);
        }

        public Task SaveAsync(Account account, CancellationToken cancellationToken = default)
        {
            _storage[account.Cpf.Value] = account;
            LastSavedAccount = account;
            return Task.CompletedTask;
        }
    }
}
