#!/bin/bash

echo "Checking Docker Compose..."

EXPECTED_COMPOSE_REGEX=^[1-9][0-9]*\.[0-9]+\.[0-9]+
DISPLAY_COMPOSE_REGEX=$(sed -e 's|\\\([.+?*()]\)|\1|g' -e 's|[.+?]\*|*|g' <<<${EXPECTED_COMPOSE_REGEX})

version=$(docker-compose version --short 2>&1)

if [[ "$version" =~ $EXPECTED_COMPOSE_REGEX ]]; then
    echo "Docker Compose version \"${version}\" (matching required \"${DISPLAY_COMPOSE_REGEX}\") is OK"
else
    echo "Current Docker Compose version is \"${version}\". Please Install Docker Compose matching \"${DISPLAY_COMPOSE_REGEX}\""
    echo "Docker Compose comes with Docker Desktop which can be downloaded here: https://www.docker.com/products/docker-desktop"
    exit 1
fi
