# Lab 6: PostgreSQL Sharding

Implementation of data sharding across 16 PostgreSQL databases using virtual nodes (vnodes) concept.

## Architecture

```
+------------------+     +------------------+
|   postgres_00    |     |   postgres_01    |
|   Port: 5440     |     |   Port: 5441     |
+------------------+     +------------------+
| db_0 | db_1      |     | db_4 | db_5      |
| db_2 | db_3      |     | db_6 | db_7      |
+------------------+     +------------------+

+------------------+     +------------------+
|   postgres_02    |     |   postgres_03    |
|   Port: 5442     |     |   Port: 5443     |
+------------------+     +------------------+
| db_8 | db_9      |     | db_c | db_d      |
| db_a | db_b      |     | db_e | db_f      |
+------------------+     +------------------+
```

## Routing Algorithm

1. Generate UUID in application (Guid.NewGuid())
2. Take the last hex character of the UUID
3. Use mapping.json to find the connection string for that character
4. Execute query on the corresponding database

Example: `plan_id = "...a1b2c3d4"` -> last char `4` -> `db_4` on `postgres_01:5441`

## Files

| File | Description |
|------|-------------|
| `docker-compose.yml` | 4 PostgreSQL services configuration |
| `mapping.json` | Shard routing (16 databases) |
| `mapping_single.json` | Single DB routing (for comparison) |
| `sql/schema.sql` | Database schema |
| `docker/postgres/init/*.sql` | Database initialization scripts |

## Quick Start

1. Start all PostgreSQL nodes:
```bash
cd sharding
docker-compose up -d
```

2. Apply schema to all 16 databases:
```bash
python cli.py apply-sql --mapping mapping.json --file sql/schema.sql
```

3. Run benchmark:
```bash
python cli.py benchmark --mapping mapping.json --count 10000 --reads 10000 --concurrency 50
```

## Database Credentials

- User: `lab6`
- Password: `lab6pwd`

## Ports

| Service | Port | Databases |
|---------|------|-----------|
| postgres_00 | 5440 | db_0, db_1, db_2, db_3 |
| postgres_01 | 5441 | db_4, db_5, db_6, db_7 |
| postgres_02 | 5442 | db_8, db_9, db_a, db_b |
| postgres_03 | 5443 | db_c, db_d, db_e, db_f |
