using System;
using System.Threading;
using System.Threading.Tasks;
using Desafio.Application.Requests;
using Desafio.Domain.Entities;
using Desafio.Domain.Repositories;

namespace Desafio.Application.Services;

public sealed class AnalystService : IAnalystService
{
    private readonly IAnalystRepository _analystRepository;

    public AnalystService(IAnalystRepository analystRepository)
    {
        _analystRepository = analystRepository ?? throw new ArgumentNullException(nameof(analystRepository));
    }

    public async Task RegisterAnalystAsync(RegisterAnalystRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var analyst = new FraudAnalyst(request.Id, request.Name, request.Role);
        await _analystRepository.SaveAsync(analyst, cancellationToken);
    }
}
