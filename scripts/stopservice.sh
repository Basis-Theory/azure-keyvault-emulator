#!/bin/bash
set -e

current_directory="$PWD"

cd $(dirname $0)/..

docker stop azure-key-vault-emulator

result=$?

cd "$current_directory"

exit $result
