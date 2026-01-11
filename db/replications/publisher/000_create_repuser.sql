-- Create replication user for logical replication
DO $$
BEGIN
    IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'replication_user') THEN
        CREATE ROLE replication_user WITH REPLICATION LOGIN PASSWORD 'replication_password';
    END IF;
END
$$;

-- Grant necessary permissions
GRANT ALL PRIVILEGES ON DATABASE "TravelerDb" TO replication_user;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO replication_user;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO replication_user;

-- Allow replication user to read all tables
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT ON TABLES TO replication_user;
