Write-Output "Building and running with docker-compose..."
docker compose build
docker compose up -d
Write-Output "Backend should be available at http://localhost:5000"
