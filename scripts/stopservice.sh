#!/bin/bash
set -e

current_directory="$PWD"

cd $(dirname $0)/..

docker stop keyvault-emulator

result=$?

cd "$current_directory"

exit $result
