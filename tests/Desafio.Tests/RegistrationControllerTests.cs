using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Desafio.Application.Requests;
using Desafio.Application.Services;
using Desafio.Controllers;
using Desafio.Domain.Entities;
using Desafio.Domain.Repositories;
using Desafio.Domain.Services;
using Desafio.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Desafio.Tests.Application.Controllers;

public class RegistrationControllerTests
{
    [Fact]
    public async Task HomeController_RegisterAnalystAsync_ReturnsCreated()
    {
        var analystRepository = new InMemoryAnalystRepository();
        var service = new AnalystService(analystRepository);
        var controller = new HomeController(new FakeLogger<HomeController>(), service);

        var request = new RegisterAnalystRequest
        {
            Id = "marisa-analyst-01",
            Name = "Marisa",
            Role = "Analista de Fraude"
        };

        var result = await controller.RegisterAnalystAsync(request);

        Assert.IsType<CreatedResult>(result);
        Assert.NotNull(analystRepository.LastSavedAnalyst);
        Assert.Equal("marisa-analyst-01", analystRepository.LastSavedAnalyst!.Id);
        Assert.Equal("Marisa", analystRepository.LastSavedAnalyst.Name);
        Assert.Equal("Analista de Fraude", analystRepository.LastSavedAnalyst.Role);
    }

    [Fact]
    public async Task AccountController_RegisterAsync_ReturnsCreatedAndSavesAccount()
    {
        var accountRepository = new InMemoryAccountRepository();
        var evaluator = new PixLimitEvaluator();
        var pixLimitService = new PixLimitService(accountRepository, evaluator);
        var accountBalanceService = new AccountBalanceService(accountRepository);
        var controller = new AccountController(pixLimitService, accountBalanceService);

        var request = new RegisterAccountRequest
        {
            Cpf = "321654987-10",
            Agency = "1234",
            Account = "1234567",
            ClientName = "David Silva"
        };

        var result = await controller.RegisterAsync(request);

        Assert.IsType<CreatedResult>(result);
        Assert.NotNull(accountRepository.LastSavedAccount);
        Assert.Equal("32165498710", accountRepository.LastSavedAccount!.Cpf.Value);
        Assert.Equal("1234", accountRepository.LastSavedAccount.Agency.Value);
        Assert.Equal("1234567", accountRepository.LastSavedAccount.Number.Value);
        Assert.Equal("David Silva", accountRepository.LastSavedAccount.ClientName);
        Assert.Equal(5000m, accountRepository.LastSavedAccount.PixLimit.Amount);
        Assert.Null(accountRepository.LastSavedAccount.LimitAudit);
    }

    private sealed class InMemoryAnalystRepository : IAnalystRepository
    {
        public FraudAnalyst? LastSavedAnalyst { get; private set; }

        public Task SaveAsync(FraudAnalyst analyst, CancellationToken cancellationToken = default)
        {
            LastSavedAnalyst = analyst;
            return Task.CompletedTask;
        }

        public Task<FraudAnalyst?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<FraudAnalyst?>(LastSavedAnalyst is not null && LastSavedAnalyst.Id == id ? LastSavedAnalyst : null);
        }
    }

    private sealed class InMemoryAccountRepository : IAccountRepository
    {
        private readonly Dictionary<string, Account> _store = new(StringComparer.OrdinalIgnoreCase);
        public Account? LastSavedAccount { get; private set; }

        public Task<Account?> GetByCpfAsync(Cpf cpf, CancellationToken cancellationToken = default)
        {
            _store.TryGetValue(cpf.Value, out var account);
            return Task.FromResult(account);
        }

        public Task SaveAsync(Account account, CancellationToken cancellationToken = default)
        {
            _store[account.Cpf.Value] = account;
            LastSavedAccount = account;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeLogger<T> : ILogger<T>
    {
        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;
        public bool IsEnabled(LogLevel logLevel) => false;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }

        private sealed class NullScope : IDisposable
        {
            public static NullScope Instance { get; } = new NullScope();
            public void Dispose() { }
        }
    }
}
