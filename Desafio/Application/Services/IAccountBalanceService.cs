using System.Threading;
using System.Threading.Tasks;
using Desafio.Application.Requests;
using Desafio.Application.Results;
using Desafio.Domain.ValueObjects;

namespace Desafio.Application.Services;

/// <summary>
/// Interface que define contrato para operações de saldo e transferências.
/// 
/// BENEFÍCIOS:
/// - Interface Segregation Principle (ISP): Clientes dependem apenas de operações de saldo
/// - Inversão de Controle (IoC): Facilita injeção de dependência e testes
/// - Substituibilidade: Permite implementações alternativas (ex: com cache, validações extras)
/// - Testabilidade: Pode ser mockada facilmente em testes unitários
/// 
/// RESPONSABILIDADES:
/// - Gerenciar depósitos
/// - Processar transferências com validações
/// - Manter consistência de saldo
/// </summary>
public interface IAccountBalanceService
{
    /// <summary>
    /// Realiza um depósito simples em uma conta.
    /// Apenas aumenta o saldo, sem impacto em limite PIX.
    /// </summary>
    Task DepositAsync(DepositRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Consulta o saldo atual da conta identificada por CPF.
    /// </summary>
    Task<Money?> GetBalanceAsync(string cpf, CancellationToken cancellationToken = default);

    /// <summary>
    /// Consulta o limite de transferência disponível para o dia atual da conta identificada por CPF.
    /// </summary>
    Task<Money?> GetAvailableDailyLimitAsync(string cpf, CancellationToken cancellationToken = default);

    /// <summary>
    /// Processa uma transferência/PIX com validações de saldo e limite diário.
    /// Retorna resultado detalhado (aprovação/negação com motivo e saldo atualizado).
    /// </summary>
    Task<TransferResult> TransferAsync(TransferRequest request, CancellationToken cancellationToken = default);
}
