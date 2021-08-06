#!/bin/bash
set -e

docker push ghcr.io/basis-theory/azure-key-vault-emulator:latest
docker push ghcr.io/basis-theory/azure-key-vault-emulator:$1
