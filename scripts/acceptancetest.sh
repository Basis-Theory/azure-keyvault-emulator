#!/bin/bash
set -e

current_directory="$PWD"

cd $(dirname $0)/..

echo "Running acceptance tests..."

dotnet test AzureKeyVaultEmulator.AcceptanceTests/AzureKeyVaultEmulator.AcceptanceTests.csproj --no-restore --no-build -c Release -v=normal

result=$?

cd "$current_directory"

docker-compose logs azure-key-vault-emulator

exit $result
