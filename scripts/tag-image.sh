#!/bin/bash
set -e

docker image tag azure-keyvault-emulator basistheory/azure-keyvault-emulator:latest
docker image tag azure-keyvault-emulator basistheory/azure-keyvault-emulator:$1
