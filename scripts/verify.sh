#!/bin/bash
set -e

# verify the software

current_directory="$PWD"

cd $(dirname $0)

time {
    ./dependencycheck.sh
    ./importcert.sh
    ./stopdocker.sh
    ./build.sh
    ./startdocker.sh
    ./serviceup.sh
    ./acceptancetest.sh
}

cd "$current_directory"
