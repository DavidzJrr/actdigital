using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Desafio.Domain.Entities;
using Desafio.Domain.Repositories;
using Desafio.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;

namespace Desafio.Infrastructure.Persistence;

public sealed class DynamoDbAccountRepository : IAccountRepository
{
    private readonly IDynamoDBContext _context;
    private readonly string _tableName;

    public DynamoDbAccountRepository(IDynamoDBContext context, IConfiguration configuration)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _tableName = configuration.GetValue<string>("Aws:TableName") ?? "BankAccounts";
    }

    public async Task<Account?> GetByCpfAsync(Cpf cpf, CancellationToken cancellationToken = default)
    {
        var operationConfig = new DynamoDBOperationConfig
        {
            OverrideTableName = _tableName,
            IndexName = "CpfIndex" // Uses a DynamoDB global secondary index on Cpf for efficient lookups
        };

        var query = _context.QueryAsync<DynamoAccountItem>(cpf.Value, operationConfig);
        var items = await query.GetRemainingAsync(cancellationToken);
        var item = items.FirstOrDefault();

        return item is null ? null : ToDomain(item);
    }

    public async Task SaveAsync(Account account, CancellationToken cancellationToken = default)
    {
        if (account is null)
        {
            throw new ArgumentNullException(nameof(account));
        }

        var item = new DynamoAccountItem
        {
            AccountId = account.AccountId.ToString(),
            Cpf = account.Cpf.Value,
            Agency = account.Agency.Value,
            Account = account.Number.Value,
            PixLimit = account.PixLimit.Amount,
            Balance = account.Balance.Amount,
            DailyLimitConsumed = account.DailyLimitConsumed.Amount,
            DailyLimitConsumedAt = account.DailyLimitConsumedAt == DateOnly.MinValue ? string.Empty : account.DailyLimitConsumedAt.ToString("yyyy-MM-dd"),
            LimitChangedById = account.LimitAudit?.Actor.Id ?? string.Empty,
            LimitChangedByName = account.LimitAudit?.Actor.Name ?? string.Empty,
            LimitChangedByRole = account.LimitAudit?.Actor.Role ?? string.Empty,
            LimitChangedAt = account.LimitAudit?.ChangedAt.ToString("o") ?? string.Empty
        };

        var operationConfig = new DynamoDBOperationConfig { OverrideTableName = _tableName };
        await _context.SaveAsync(item, operationConfig, cancellationToken);
    }

    private static Account ToDomain(DynamoAccountItem item)
    {
        LimitAudit? audit = null;

        if (!string.IsNullOrWhiteSpace(item.LimitChangedById) && !string.IsNullOrWhiteSpace(item.LimitChangedByName) && !string.IsNullOrWhiteSpace(item.LimitChangedByRole) && DateTimeOffset.TryParse(item.LimitChangedAt, out var changedAt))
        {
            var analyst = new FraudAnalyst(item.LimitChangedById, item.LimitChangedByName, item.LimitChangedByRole);
            audit = new LimitAudit(analyst, changedAt);
        }

        var consumedAt = DateOnly.MinValue;
        if (!string.IsNullOrWhiteSpace(item.DailyLimitConsumedAt))
        {
            DateOnly.TryParse(item.DailyLimitConsumedAt, out consumedAt);
        }

        return new Account(
            new Cpf(item.Cpf),
            new AgencyNumber(item.Agency),
            new AccountNumber(item.Account),
            new Money(item.PixLimit),
            audit,
            Guid.Parse(item.AccountId),
            new Money(item.Balance),
            new Money(item.DailyLimitConsumed),
            consumedAt);
    }

    [DynamoDBTable("BankAccounts")]
    private sealed class DynamoAccountItem
    {
        [DynamoDBHashKey("AccountId")]
        public string AccountId { get; set; } = string.Empty;

        [DynamoDBProperty("Cpf")]
        [DynamoDBGlobalSecondaryIndexHashKey("CpfIndex")] // Global secondary index for CPF-based account lookup
        public string Cpf { get; set; } = string.Empty;

        [DynamoDBProperty("Agency")]
        public string Agency { get; set; } = string.Empty;

        [DynamoDBProperty("Account")]
        public string Account { get; set; } = string.Empty;

        [DynamoDBProperty("PixLimit")]
        public decimal PixLimit { get; set; }

        [DynamoDBProperty("Balance")]
        public decimal Balance { get; set; }

        [DynamoDBProperty("DailyLimitConsumed")]
        public decimal DailyLimitConsumed { get; set; }

        [DynamoDBProperty("DailyLimitConsumedAt")]
        public string DailyLimitConsumedAt { get; set; } = string.Empty;

        [DynamoDBProperty("LimitChangedById")]
        public string LimitChangedById { get; set; } = string.Empty;

        [DynamoDBProperty("LimitChangedByName")]
        public string LimitChangedByName { get; set; } = string.Empty;

        [DynamoDBProperty("LimitChangedByRole")]
        public string LimitChangedByRole { get; set; } = string.Empty;

        [DynamoDBProperty("LimitChangedAt")]
        public string LimitChangedAt { get; set; } = string.Empty;
    }
}
