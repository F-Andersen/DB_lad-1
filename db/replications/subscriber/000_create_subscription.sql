-- Create subscription to master database
-- This connects to the publisher and subscribes to changes

DO $$
BEGIN
    IF NOT EXISTS (SELECT FROM pg_subscription WHERE subname = 'traveler_subscription') THEN
        CREATE SUBSCRIPTION traveler_subscription
        CONNECTION 'host=travelerdb port=5432 dbname=TravelerDb user=replication_user password=replication_password'
        PUBLICATION traveler_publication
        WITH (copy_data = true, create_slot = true);
    END IF;
END
$$;

-- Verify subscription created
SELECT * FROM pg_subscription;
