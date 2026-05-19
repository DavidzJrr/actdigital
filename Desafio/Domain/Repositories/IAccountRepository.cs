using System.Threading;
using System.Threading.Tasks;
using Desafio.Domain.Entities;
using Desafio.Domain.ValueObjects;

namespace Desafio.Domain.Repositories;

public interface IAccountRepository
{
    Task<Account?> GetByCpfAsync(Cpf cpf, CancellationToken cancellationToken = default);

    Task SaveAsync(Account account, CancellationToken cancellationToken = default);
}
