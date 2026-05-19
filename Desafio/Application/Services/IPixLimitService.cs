using System.Threading;
using System.Threading.Tasks;
using Desafio.Application.Requests;
using Desafio.Application.Results;

namespace Desafio.Application.Services;

/// <summary>
/// Interface que define contrato para operações de limite PIX.
/// 
/// BENEFÍCIOS:
/// - Interface Segregation Principle (ISP): Clientes dependem apenas de operações PIX
/// - Inversão de Controle (IoC): Facilita injeção de dependência e testes
/// - Substituibilidade: Permite implementações alternativas sem alterar código cliente
/// 
/// RESPONSABILIDADES:
/// - Validar transações contra limite PIX
/// - Registrar novas contas
/// - Atualizar limites (com auditoria)
/// </summary>
public interface IPixLimitService
{
    /// <summary>
    /// Valida se uma transação PIX é permitida dentro dos limites configurados.
    /// Retorna resultado detalhado (aprovação/negação com motivo).
    /// </summary>
    Task<PixLimitResult> ValidateTransactionAsync(PixLimitRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra uma nova conta com limite PIX padrão.
    /// </summary>
    Task RegisterAccountAsync(RegisterAccountRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza o limite PIX de uma conta existente (geralmente por analista de fraude).
    /// Registra auditoria de quem alterou e quando.
    /// </summary>
    Task UpdateAccountLimitAsync(UpdateAccountLimitRequest request, CancellationToken cancellationToken = default);
}
