#!/bin/bash

RED="\033[1;31m"
GREEN="\033[1;32m"
NOCOLOR="\033[0m"

i=0
timeout=30000

# continue until $n equals 5
while [ $i -le $timeout ]
do
	status=$(curl -s -o /dev/null -i -w "%{http_code}" https://localhost:5001/)

    if [ $status == "404" ]
    then
        echo -e "${GREEN}✓${NOCOLOR} KeyVault Emulator is ready"
        exit 0
    else
        echo -e "❌ KeyVault Emulator is not ready"

        sleep 3
	    i=$(( i+3000 ))	 # increments $n
    fi
done
docker-compose logs azure-key-vault-emulator
echo 'Health check did not pass within timeout'
exit 1
