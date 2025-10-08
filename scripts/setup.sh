#!/usr/bin/env bash
set -e
echo "Building and running with docker-compose..."
docker compose build
docker compose up -d
echo "Backend should be available at http://localhost:5000"
