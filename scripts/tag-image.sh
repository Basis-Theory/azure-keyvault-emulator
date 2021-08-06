#!/bin/bash
set -e

docker image tag azure-key-vault-emulator ghcr.io/basis-theory/azure-key-vault-emulator:latest
docker image tag azure-key-vault-emulator ghcr.io/basis-theory/azure-key-vault-emulator:$1
