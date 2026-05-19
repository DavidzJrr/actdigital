# Desafio

Aplicação ASP.NET Core 8 que implementa um serviço bancário básico com validação de limite PIX, cadastro de contas, depósito e transferência.

## Visão geral

O projeto segue uma arquitetura em camadas:

- `Domain`: entidades de negócio, repositórios e serviços de avaliação de limite.
- `Application`: serviços de aplicação e contratos de requisição/resultado (DTOs).
- `Infrastructure`: persistência com DynamoDB usando AWS SDK.
- `Controllers`: API REST para operações de conta.

A solução também possui uma interface MVC padrão para páginas simples e Swagger habilitado em desenvolvimento.

## Funcionalidades principais

- Registrar conta bancária por CPF, agência e número da conta.
- Validar transações PIX contra o limite atual da conta.
- Atualizar o limite PIX da conta com auditoria de analista de fraude.
- Depositar saldo na conta.
- Realizar transferências respeitando saldo e limite diário.

## Tecnologias

- .NET 8.0
- ASP.NET Core MVC / API
- AWS SDK para DynamoDB (`AWSSDK.DynamoDBv2`)
- Swagger (`Swashbuckle.AspNetCore`)

## Estrutura do projeto

- `Program.cs`: configuração de services, pipeline e Swagger.
- `Controllers/AccountController.cs`: expõe os endpoints da API.
- `Application/Services`: lógica de aplicação e orquestração de casos de uso.
- `Domain/Entities`: modelo de domínio com `Account`, regras de negócio e objetos de valor.
- `Infrastructure/Persistence`: repositórios DynamoDB.

## Endpoints principais

A API está exposta em `api/Account` com os seguintes recursos:

- `POST api/Account/register` - cadastra nova conta.
- `POST api/Account/validate-transaction` - valida transação PIX.
- `POST api/Account/set-limit` - atualiza limite PIX.
- `POST api/Account/deposit` - adiciona saldo à conta.
- `POST api/Account/transfer` - executa transferência bancária.

## Configuração

No `appsettings.json` ou `appsettings.Development.json`, configure:

```json
"Aws": {
  "Region": "us-east-1",
  "TableName": "BankAccounts"
}
```

O repositório DynamoDB espera uma tabela `BankAccounts` com um índice global secundário `CpfIndex` para consultas por CPF.

## Como executar

1. Restaurar pacotes:

```bash
dotnet restore
```

2. Build:

```bash
dotnet build
```

3. Executar:

```bash
dotnet run
```

4. Em desenvolvimento, Swagger estará disponível em `https://localhost:{port}/swagger`.

## Observações

- O projeto está configurado para usar `IAmazonDynamoDB` e `IDynamoDBContext` como serviços singleton.
- O serviço de validação de limite PIX usa regras de domínio para avaliar se uma transferência está dentro do limite.
- A aplicação mantém histórico de alteração de limite com identificação do analista.
