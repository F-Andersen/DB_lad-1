#!/bin/bash
set -e

echo "=== Running orchestration script ==="

# Run migrations if they exist
if [ -d "/docker-entrypoint-migrations" ]; then
    echo "Running migrations..."
    for f in /docker-entrypoint-migrations/*.sql; do
        if [ -f "$f" ]; then
            echo "Executing $f"
            psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" -f "$f"
        fi
    done
fi

# Run replication scripts if they exist
if [ -d "/docker-entrypoint-replications" ]; then
    echo "Running replication scripts..."
    for f in /docker-entrypoint-replications/*.sql; do
        if [ -f "$f" ]; then
            echo "Executing $f"
            psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" -f "$f"
        fi
    done
fi

echo "=== Orchestration complete ==="
