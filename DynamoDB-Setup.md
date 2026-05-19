# DynamoDB Setup

Este repositório usa DynamoDB para persistir contas bancárias e analistas.

## Configuração do projeto

Em `Desafio/Desafio/appsettings.json`, a aplicação já está configurada para:

- Região: `us-east-1`
- Tabela de contas: `BankAccounts`
- Tabela de analistas: `FraudAnalysts`

```json
"Aws": {
  "Region": "us-east-1",
  "TableName": "BankAccounts",
  "AnalystTableName": "FraudAnalysts"
}
```

## Pré-requisitos

- AWS CLI instalado
- Credenciais AWS configuradas (`aws configure` ou variáveis de ambiente)
- Permissão para criar tabelas DynamoDB

## Tabela `BankAccounts`

A tabela principal deve ter:

- Partition key: `AccountId` (String)
- GSI: `CpfIndex`
  - Partition key: `Cpf` (String)
  - Projection: `ALL`

A aplicação salva campos extras no item, como:

- `Agency`
- `Account`
- `PixLimit`
- `LimitChangedById`
- `LimitChangedByName`
- `LimitChangedByRole`
- `LimitChangedAt`

## Tabela `FraudAnalysts`

A tabela de analistas deve ter:

- Partition key: `Id` (String)

## Criar tabelas com AWS CLI

Use os scripts em `scripts/` ou execute os comandos abaixo.

### Comando para `BankAccounts`

```bash
aws dynamodb create-table \
  --table-name BankAccounts \
  --attribute-definitions AttributeName=AccountId,AttributeType=S AttributeName=Cpf,AttributeType=S \
  --key-schema AttributeName=AccountId,KeyType=HASH \
  --billing-mode PAY_PER_REQUEST \
  --global-secondary-indexes 'IndexName=CpfIndex,KeySchema=[{AttributeName=Cpf,KeyType=HASH}],Projection={ProjectionType=ALL}'
```

### Comando para `FraudAnalysts`

```bash
aws dynamodb create-table \
  --table-name FraudAnalysts \
  --attribute-definitions AttributeName=Id,AttributeType=S \
  --key-schema AttributeName=Id,KeyType=HASH \
  --billing-mode PAY_PER_REQUEST
```

## Scripts de criação

- `scripts/create-dynamodb-tables.sh` — script Bash
- `scripts/create-dynamodb-tables.ps1` — script PowerShell

## Executar a aplicação

Após criar as tabelas, execute a aplicação no diretório do projeto:

```bash
cd Desafio/Desafio
dotnet run
```

Se você estiver na pasta `Desafio`, use:

```bash
dotnet run --project Desafio/Desafio.csproj
```

## Observação

A tabela `BankAccounts` precisa do índice secundário global `CpfIndex` para que o repositório faça lookup por CPF sem scans.
