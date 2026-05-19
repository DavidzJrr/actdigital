#!/usr/bin/env bash
set -euo pipefail

REGION="${AWS_REGION:-us-east-1}"

echo "Creating DynamoDB tables in region ${REGION}..."

aws dynamodb create-table \
  --region "${REGION}" \
  --table-name BankAccounts \
  --attribute-definitions AttributeName=AccountId,AttributeType=S AttributeName=Cpf,AttributeType=S \
  --key-schema AttributeName=AccountId,KeyType=HASH \
  --billing-mode PAY_PER_REQUEST \
  --global-secondary-indexes 'IndexName=CpfIndex,KeySchema=[{AttributeName=Cpf,KeyType=HASH}],Projection={ProjectionType=ALL}'

aws dynamodb create-table \
  --region "${REGION}" \
  --table-name FraudAnalysts \
  --attribute-definitions AttributeName=Id,AttributeType=S \
  --key-schema AttributeName=Id,KeyType=HASH \
  --billing-mode PAY_PER_REQUEST

echo "DynamoDB tables created successfully."
