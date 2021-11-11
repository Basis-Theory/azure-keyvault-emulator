#!/bin/bash
set -e

docker image tag basistheory/azure-keyvault-emulator basistheory/azure-keyvault-emulator:latest
docker image tag basistheory/azure-keyvault-emulator basistheory/azure-keyvault-emulator:$1
