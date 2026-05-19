param(
    [string]$Region = 'us-east-1'
)

Write-Host "Creating DynamoDB tables in region $Region..."

aws dynamodb create-table `
  --region $Region `
  --table-name BankAccounts `
  --attribute-definitions AttributeName=AccountId,AttributeType=S AttributeName=Cpf,AttributeType=S `
  --key-schema AttributeName=AccountId,KeyType=HASH `
  --billing-mode PAY_PER_REQUEST `
  --global-secondary-indexes 'IndexName=CpfIndex,KeySchema=[{AttributeName=Cpf,KeyType=HASH}],Projection={ProjectionType=ALL}'

aws dynamodb create-table `
  --region $Region `
  --table-name FraudAnalysts `
  --attribute-definitions AttributeName=Id,AttributeType=S `
  --key-schema AttributeName=Id,KeyType=HASH `
  --billing-mode PAY_PER_REQUEST

Write-Host "DynamoDB tables created successfully."
