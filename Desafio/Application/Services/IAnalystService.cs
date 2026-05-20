using System.Threading;
using System.Threading.Tasks;
using Desafio.Application.Requests;
using Desafio.Application.Results;

namespace Desafio.Application.Services;

/// <summary>
/// Interface que define contrato para operações de analistas.
/// </summary>
public interface IAnalystService
{
    Task RegisterAnalystAsync(RegisterAnalystRequest request, CancellationToken cancellationToken = default);
}
