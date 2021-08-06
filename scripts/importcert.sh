#!/bin/bash
set -e

current_directory="$PWD"

cd $(dirname $0)/..

if [ "$(uname)" == "Darwin" ]; then
    sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain local-certs/azure-key-vault-emulator.crt
elif [ "$(expr substr $(uname -s) 1 5)" == "Linux" ]; then
    sudo cp local-certs/azure-key-vault-emulator.crt /usr/local/share/ca-certificates/azure-key-vault-emulator.crt
    sudo update-ca-certificates
fi

result=$?

cd "$current_directory"

exit $result
