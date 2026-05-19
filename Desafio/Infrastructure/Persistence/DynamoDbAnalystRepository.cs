using Amazon.DynamoDBv2.DataModel;
using Desafio.Domain.Entities;
using Desafio.Domain.Repositories;

namespace Desafio.Infrastructure.Persistence;

public sealed class DynamoDbAnalystRepository : IAnalystRepository
{
    private readonly IDynamoDBContext _context;
    private readonly string _tableName;

    public DynamoDbAnalystRepository(IDynamoDBContext context, IConfiguration configuration)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _tableName = configuration.GetValue<string>("Aws:AnalystTableName") ?? "FraudAnalysts";
    }

    public async Task SaveAsync(FraudAnalyst analyst, CancellationToken cancellationToken = default)
    {
        if (analyst is null)
        {
            throw new ArgumentNullException(nameof(analyst));
        }

        var item = new DynamoAnalystItem
        {
            Id = analyst.Id,
            Name = analyst.Name,
            Role = analyst.Role
        };

        var operationConfig = new DynamoDBOperationConfig { OverrideTableName = _tableName };
        await _context.SaveAsync(item, operationConfig, cancellationToken);
    }

    public async Task<FraudAnalyst?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var operationConfig = new DynamoDBOperationConfig { OverrideTableName = _tableName };
        var item = await _context.LoadAsync<DynamoAnalystItem>(id, operationConfig, cancellationToken);
        return item is null ? null : new FraudAnalyst(item.Id, item.Name, item.Role);
    }

    [DynamoDBTable("FraudAnalysts")]
    private sealed class DynamoAnalystItem
    {
        [DynamoDBHashKey("Id")]
        public string Id { get; set; } = string.Empty;

        [DynamoDBProperty("Name")]
        public string Name { get; set; } = string.Empty;

        [DynamoDBProperty("Role")]
        public string Role { get; set; } = string.Empty;
    }
}
