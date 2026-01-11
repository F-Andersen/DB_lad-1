-- Schema for travel plans sharding
-- This script should be applied to all 16 databases

CREATE TABLE IF NOT EXISTS travel_plans (
    id uuid PRIMARY KEY,
    title text NOT NULL,
    created_at timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS travel_locations (
    plan_id uuid PRIMARY KEY REFERENCES travel_plans(id) ON DELETE CASCADE,
    country text NOT NULL,
    city text NOT NULL,
    address text NULL,
    updated_at timestamptz NOT NULL DEFAULT now()
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_travel_plans_created_at ON travel_plans(created_at);
CREATE INDEX IF NOT EXISTS idx_travel_locations_updated_at ON travel_locations(updated_at);
