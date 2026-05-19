using System.Threading;
using System.Threading.Tasks;
using Desafio.Domain.Entities;

namespace Desafio.Domain.Repositories;

public interface IAnalystRepository
{
    Task SaveAsync(FraudAnalyst analyst, CancellationToken cancellationToken = default);
    Task<FraudAnalyst?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}
