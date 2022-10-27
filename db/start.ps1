docker build -t mssql-countryip -f .\Dockerfile --no-cache .
docker-compose -f .\docker-compose.yaml down
docker volume prune --force
docker compose  -f .\docker-compose.yaml up -d 