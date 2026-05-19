namespace Desafio.Domain.Entities;

/// <summary>
/// Entidade que representa uma ação do sistema (não um ator humano).
/// Usada para auditoria quando operações são executadas automaticamente pelo sistema.
/// 
/// PADRÕES APLICADOS:
/// - Entity Pattern: Tem identidade única
/// - Singleton Pattern: Instância única reutilizável (não precisa de nova instância)
/// - Liskov Substitution Principle: Implementa IActor de forma semântica correta
/// 
/// Por que existe?
/// - Operações do sistema precisam de auditoria (quem executou)
/// - LimitConsumptionEvent era uma violação de LSP (evento != ator)
/// - SystemActor é um "ator especial" que representa o próprio sistema
/// 
/// RESPONSABILIDADES:
/// - Identificar ações executadas pelo sistema (não por pessoas)
/// - Fornecer informações para auditoria (sistema, não analista)
/// 
/// EXEMPLO DE USO:
/// ```csharp
/// var actor = new SystemActor();
/// account.ConsumeDailyLimit(amount, actor, DateTimeOffset.UtcNow);
/// ```
/// 
/// Na auditoria, verá: "Sistema", "Consumo de Limite Diário", "System"
/// em vez de uma pessoa específica.
/// </summary>
public sealed class SystemActor : IActor
{
    /// <summary>ID fixo que identifica o sistema.</summary>
    public string Id => "system";
    
    /// <summary>Nome que identifica a origem como sistema.</summary>
    public string Name => "Sistema";
    
    /// <summary>Função/papel do ator de sistema.</summary>
    public string Role => "SystemProcess";

    /// <summary>
    /// Construtor sem parâmetros.
    /// SystemActor é geralmente uma instância única, pode ser reutilizada.
    /// </summary>
    public SystemActor()
    {
    }
}
