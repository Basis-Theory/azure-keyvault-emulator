#!/bin/bash
set -e

docker push basistheory/azure-keyvault-emulator:latest
docker push basistheory/azure-keyvault-emulator:$1
