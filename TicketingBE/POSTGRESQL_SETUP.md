# PostgreSQL Setup Guide

## ✅ Migration to PostgreSQL Complete!

Your backend has been successfully migrated from SQL Server to PostgreSQL.

## 📦 Package Changes

- **Removed**: `System.Data.SqlClient` (SQL Server)
- **Added**: `Npgsql` version 8.0.5 (PostgreSQL)

## 🔧 Configuration Updates

### Connection Strings

**appsettings.json** (Production):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=TicketingDB;Username=YOUR_USERNAME;Password=YOUR_PASSWORD;"
  }
}
```

**appsettings.Development.json** (Development):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=TicketingDB;Username=postgres;Password=postgres;"
  }
}
```

### Connection String Parameters

- **Host**: PostgreSQL server address (e.g., localhost, 192.168.1.100)
- **Port**: PostgreSQL port (default: 5432)
- **Database**: Database name
- **Username**: PostgreSQL username
- **Password**: PostgreSQL password

**Optional Parameters**:
```
Host=localhost;Port=5432;Database=TicketingDB;Username=postgres;Password=postgres;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=100;Connection Lifetime=0;
```

## 🗄️ Database Setup

### 1. Create Database

Connect to PostgreSQL using psql:
```bash
psql -U postgres
```

Create the database:
```sql
CREATE DATABASE "TicketingDB";
```

Exit and connect to the new database:
```bash
\q
psql -U postgres -d TicketingDB
```

### 2. Run Setup Script

Run the `DatabaseSetup.sql` script:
```bash
psql -U postgres -d TicketingDB -f DatabaseSetup.sql
```

Or copy and paste the contents into pgAdmin or your PostgreSQL client.

## 📋 Key Changes from SQL Server

### SQL Syntax Differences

| Feature | SQL Server | PostgreSQL |
|---------|-----------|------------|
| Auto-increment | `IDENTITY(1,1)` | `SERIAL` |
| Boolean | `BIT` (0/1) | `BOOLEAN` (true/false) |
| String | `NVARCHAR` | `VARCHAR` |
| DateTime | `DATETIME2` | `TIMESTAMP` |
| Get inserted ID | `SCOPE_IDENTITY()` | `RETURNING id` |
| Current time | `GETUTCDATE()` | `CURRENT_TIMESTAMP` |
| Case sensitivity | Case-insensitive | Case-sensitive (use lowercase) |
| Parameter prefix | `@param` | `@param` (same) |

### Table and Column Naming

PostgreSQL convention uses **lowercase with underscores**:
- SQL Server: `Users`, `UserId`, `FullName`
- PostgreSQL: `users`, `user_id`, `full_name`

**Users Table Schema**:
```sql
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(100) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    full_name VARCHAR(200) NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);
```

## 🔌 SqlHelper Changes

The `SqlHelper` class now uses **Npgsql** types:

### Parameter Types

```csharp
// Npgsql parameter types
NpgsqlDbType.Integer      // int
NpgsqlDbType.Bigint       // long
NpgsqlDbType.Varchar      // string
NpgsqlDbType.Timestamp    // DateTime
NpgsqlDbType.Boolean      // bool
NpgsqlDbType.Uuid         // Guid
NpgsqlDbType.Bytea        // byte[]
NpgsqlDbType.Numeric      // decimal

// Arrays (PostgreSQL specific)
NpgsqlDbType.Array | NpgsqlDbType.Varchar   // string[]
NpgsqlDbType.Array | NpgsqlDbType.Uuid      // Guid[]
```

### Usage Example

```csharp
string query = "SELECT id, username, email FROM users WHERE id = @id";

var user = _sqlHelper.ExecuteReader<User?>(query, (dataReader) =>
{
    User? user = null;
    if (dataReader.Read())
    {
        user = new User
        {
            Id = TypeHelper.GetInt32(dataReader["id"]),
            Username = TypeHelper.GetString(dataReader["username"]),
            Email = TypeHelper.GetString(dataReader["email"])
        };
    }
    return user;
}, _sqlHelper.CreateParam("@id", userId));
```

## 🚀 Quick Start

### 1. Install PostgreSQL

**Windows**:
- Download from: https://www.postgresql.org/download/windows/
- Or use Docker: `docker run --name postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:16`

**Linux**:
```bash
sudo apt update
sudo apt install postgresql postgresql-contrib
```

**macOS**:
```bash
brew install postgresql@16
brew services start postgresql@16
```

### 2. Verify Installation

```bash
psql --version
```

### 3. Update Connection String

Edit `appsettings.Development.json` with your credentials.

### 4. Create Database

```bash
psql -U postgres -c "CREATE DATABASE \"TicketingDB\";"
```

### 5. Run Database Setup Script

```bash
psql -U postgres -d TicketingDB -f DatabaseSetup.sql
```

### 6. Restore NuGet Packages

```bash
dotnet restore
```

### 7. Build Project

```bash
dotnet build
```

### 8. Run Application

```bash
dotnet run
```

## 🔍 Verify Setup

Test the database connection:
```bash
psql -U postgres -d TicketingDB -c "SELECT * FROM users;"
```

Expected output:
```
 id |   username    |           email           |    full_name     |         created_at         | is_active
----+---------------+---------------------------+------------------+----------------------------+-----------
  1 | john.doe      | john.doe@example.com      | John Doe         | 2025-10-31 12:00:00        | t
  2 | jane.smith    | jane.smith@example.com    | Jane Smith       | 2025-10-31 12:00:00        | t
  ...
```

## 🛠️ Useful PostgreSQL Commands

```sql
-- List all databases
\l

-- Connect to a database
\c TicketingDB

-- List all tables
\dt

-- Describe table structure
\d users

-- Show all users
SELECT * FROM users;

-- Show table sizes
\dt+

-- Exit psql
\q
```

## 📊 Common Queries

```sql
-- Count users
SELECT COUNT(*) FROM users;

-- Find active users
SELECT * FROM users WHERE is_active = TRUE;

-- Search by email
SELECT * FROM users WHERE email LIKE '%@example.com';

-- Update user
UPDATE users SET username = 'new_username' WHERE id = 1;

-- Delete user
DELETE FROM users WHERE id = 1;
```

## 🔒 Security Best Practices

1. **Never commit passwords** to source control
2. Use **environment variables** for production:
   ```bash
   export ConnectionStrings__DefaultConnection="Host=prod-server;..."
   ```
3. Create a **dedicated database user** (not postgres):
   ```sql
   CREATE USER ticketing_app WITH PASSWORD 'secure_password';
   GRANT ALL PRIVILEGES ON DATABASE "TicketingDB" TO ticketing_app;
   GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO ticketing_app;
   GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO ticketing_app;
   ```

## 🐞 Troubleshooting

### Connection Failed

**Error**: `Connection refused`
- Check PostgreSQL is running: `sudo systemctl status postgresql`
- Verify port 5432 is open
- Check `pg_hba.conf` for connection permissions

**Error**: `Password authentication failed`
- Verify username and password
- Check `pg_hba.conf` authentication method

### Case Sensitivity Issues

PostgreSQL is case-sensitive for identifiers. Always use lowercase:
```sql
-- ✅ Correct
SELECT id FROM users WHERE username = 'john';

-- ❌ Wrong (will fail if using lowercase table)
SELECT Id FROM Users WHERE Username = 'john';
```

### Data Type Mismatch

Make sure parameter types match:
```csharp
// ✅ Correct - boolean for PostgreSQL
_sqlHelper.CreateParam("@is_active", true);

// ❌ Wrong - int (SQL Server BIT style)
_sqlHelper.CreateParam("@is_active", 1);
```

## 📚 Additional Resources

- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Npgsql Documentation](https://www.npgsql.org/doc/)
- [PostgreSQL Tutorial](https://www.postgresqltutorial.com/)

---

**Your application is now fully configured to use PostgreSQL!** 🎉
