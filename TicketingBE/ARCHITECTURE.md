# Ticketing Backend - Three-Tier Architecture

## 📁 Project Structure

```
TicketingBE/
├── Controllers/          # API Controllers (Presentation Layer)
│   └── UserController.cs
├── BLL/                  # Business Logic Layer
│   ├── Interfaces/
│   │   └── IUserService.cs
│   └── Services/
│       └── UserService.cs
├── DAL/                  # Data Access Layer
│   ├── Interfaces/
│   │   └── IUserRepository.cs
│   └── Repositories/
│       └── UserRepository.cs
├── Models/               # Data Models/Entities
│   └── User.cs
├── Program.cs            # Application entry point with DI configuration
└── appsettings.json      # Configuration including connection strings
```

## 🏗️ Architecture Overview

This project follows the **Three-Tier Architecture** pattern:

### 1. **Presentation Layer (Controllers)**
- Handles HTTP requests and responses
- Validates input data from API calls
- Returns appropriate HTTP status codes
- Communicates **only** with the Business Logic Layer

### 2. **Business Logic Layer (BLL)**
- Contains core business rules and validation logic
- Handles data transformation and business workflows
- Coordinates between Controllers and Data Access Layer
- Implements service interfaces for dependency injection

### 3. **Data Access Layer (DAL)**
- Handles all database operations using **ADO.NET**
- Executes SQL queries and commands
- Maps database results to domain models
- No direct database access from other layers

## 🔄 Data Flow

```
HTTP Request → Controller → BLL Service → DAL Repository → Database
                    ↓            ↓              ↓
HTTP Response ← Controller ← BLL Service ← DAL Repository ← Database
```

## 💉 Dependency Injection

All services are registered in `Program.cs`:

```csharp
// DAL - Data Access Layer
builder.Services.AddScoped<IUserRepository, UserRepository>();

// BLL - Business Logic Layer
builder.Services.AddScoped<IUserService, UserService>();
```

## 🗄️ Database Setup

### SQL Server Database Creation

Run the following SQL script to create the database and Users table:

```sql
-- Create Database
CREATE DATABASE TicketingDB;
GO

USE TicketingDB;
GO

-- Create Users Table
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    FullName NVARCHAR(200) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsActive BIT NOT NULL DEFAULT 1
);
GO

-- Create Index on Email for faster lookups
CREATE INDEX IX_Users_Email ON Users(Email);
GO

-- Insert Sample Data
INSERT INTO Users (Username, Email, FullName, CreatedAt, IsActive)
VALUES 
    ('john.doe', 'john.doe@example.com', 'John Doe', GETUTCDATE(), 1),
    ('jane.smith', 'jane.smith@example.com', 'Jane Smith', GETUTCDATE(), 1),
    ('bob.johnson', 'bob.johnson@example.com', 'Bob Johnson', GETUTCDATE(), 1);
GO
```

### Connection String Configuration

Update the connection string in `appsettings.json` or `appsettings.Development.json`:

**For SQL Server Authentication:**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=TicketingDB;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
}
```

**For Windows Authentication (Development):**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=TicketingDB;Integrated Security=True;TrustServerCertificate=True;"
}
```

## 🚀 API Endpoints

### User Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/User` | Get all active users |
| GET | `/api/User/{id}` | Get user by ID |
| GET | `/api/User/email/{email}` | Get user by email |
| POST | `/api/User` | Create new user |
| PUT | `/api/User/{id}` | Update existing user |
| DELETE | `/api/User/{id}/deactivate` | Soft delete (deactivate) user |
| DELETE | `/api/User/{id}` | Permanently delete user |

## 📝 Example Usage

### Create User
```json
POST /api/User
{
  "username": "newuser",
  "email": "newuser@example.com",
  "fullName": "New User"
}
```

### Update User
```json
PUT /api/User/1
{
  "id": 1,
  "username": "updateduser",
  "email": "updated@example.com",
  "fullName": "Updated User",
  "isActive": true
}
```

## 🛠️ Running the Application

1. **Update connection string** in `appsettings.json`
2. **Create database** using the SQL script above
3. **Build the project**:
   ```bash
   dotnet build
   ```
4. **Run the application**:
   ```bash
   dotnet run
   ```
5. **Access Swagger UI**: Navigate to `https://localhost:{port}/swagger`

## 📦 Dependencies

- **.NET 8.0**
- **System.Data.SqlClient** (for ADO.NET)
- **Swashbuckle.AspNetCore** (for Swagger/OpenAPI)

## ✅ Best Practices Implemented

- ✅ Clean separation of concerns (three layers)
- ✅ Interface-based programming for testability
- ✅ Dependency injection for loose coupling
- ✅ Async/await for improved performance
- ✅ Comprehensive error handling and logging
- ✅ Input validation at multiple layers
- ✅ Business rule enforcement in BLL
- ✅ Parameterized SQL queries (SQL injection prevention)
- ✅ Soft delete pattern (deactivate instead of delete)
- ✅ RESTful API conventions

## 🔐 Security Considerations

- Always use parameterized queries to prevent SQL injection
- Store connection strings in environment variables or secure vaults in production
- Implement authentication and authorization as needed
- Use HTTPS in production environments

## 📚 Adding New Features

To add a new entity (e.g., Ticket):

1. **Create Model**: Add `Ticket.cs` in `Models/` folder
2. **Create DAL**: Add `ITicketRepository.cs` and `TicketRepository.cs`
3. **Create BLL**: Add `ITicketService.cs` and `TicketService.cs`
4. **Create Controller**: Add `TicketController.cs`
5. **Register DI**: Add registrations in `Program.cs`
6. **Create Database Table**: Run SQL script to create table

## 📄 License

This project is a boilerplate template for building three-tier architecture applications.
