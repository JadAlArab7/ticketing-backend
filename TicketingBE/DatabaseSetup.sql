-- =============================================
-- Ticketing Backend Database Setup Script (PostgreSQL)
-- =============================================

-- Note: Run this script using psql or pgAdmin
-- To create the database, connect to postgres database first:
-- CREATE DATABASE "TicketingDB";
-- Then connect to TicketingDB and run the rest of the script

-- =============================================
-- Create Users Table
-- =============================================
CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(100) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    full_name VARCHAR(200) NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    
    CONSTRAINT chk_users_email CHECK (email ~ '^[^@]+@[^@]+\.[^@]+$')
);

-- =============================================
-- Create Indexes
-- =============================================
CREATE INDEX IF NOT EXISTS ix_users_email ON users(email);
CREATE INDEX IF NOT EXISTS ix_users_is_active ON users(is_active);

-- =============================================
-- Insert Sample Data
-- =============================================
INSERT INTO users (username, email, full_name, created_at, is_active)
VALUES 
    ('john.doe', 'john.doe@example.com', 'John Doe', CURRENT_TIMESTAMP, TRUE),
    ('jane.smith', 'jane.smith@example.com', 'Jane Smith', CURRENT_TIMESTAMP, TRUE),
    ('bob.johnson', 'bob.johnson@example.com', 'Bob Johnson', CURRENT_TIMESTAMP, TRUE),
    ('alice.williams', 'alice.williams@example.com', 'Alice Williams', CURRENT_TIMESTAMP, TRUE),
    ('charlie.brown', 'charlie.brown@example.com', 'Charlie Brown', CURRENT_TIMESTAMP, FALSE)
ON CONFLICT (email) DO NOTHING;

-- =============================================
-- Verify Setup
-- =============================================
SELECT 
    'Database Setup Complete' AS status,
    COUNT(*) AS total_users,
    SUM(CASE WHEN is_active = TRUE THEN 1 ELSE 0 END) AS active_users,
    SUM(CASE WHEN is_active = FALSE THEN 1 ELSE 0 END) AS inactive_users
FROM users;
