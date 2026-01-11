-- Create publication for all tables
-- This publishes INSERT, UPDATE, DELETE operations

DO $$
BEGIN
    IF NOT EXISTS (SELECT FROM pg_publication WHERE pubname = 'traveler_publication') THEN
        CREATE PUBLICATION traveler_publication FOR ALL TABLES;
    END IF;
END
$$;

-- Verify publication created
SELECT * FROM pg_publication;
