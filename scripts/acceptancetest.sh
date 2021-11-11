#!/bin/bash
set -e

current_directory="$PWD"

cd $(dirname $0)/..

echo "Running acceptance tests..."

dotnet restore -v=quiet
dotnet build AzureKeyVaultEmulator.AcceptanceTests/AzureKeyVaultEmulator.AcceptanceTests.csproj --no-restore -c Release -v=minimal
dotnet test AzureKeyVaultEmulator.AcceptanceTests/AzureKeyVaultEmulator.AcceptanceTests.csproj --no-restore --no-build -c Release -v=normal

result=$?

cd "$current_directory"

exit $result
