# Quick Reference: Three-Tier Architecture Pattern

## 🎯 Layer Responsibilities

### **Controllers (Presentation Layer)**
```csharp
// RESPONSIBILITIES:
// ✓ Handle HTTP requests/responses
// ✓ Validate route parameters
// ✓ Return appropriate status codes
// ✓ Call BLL services
// ✗ NO business logic
// ✗ NO direct database access
```

### **BLL - Services (Business Logic Layer)**
```csharp
// RESPONSIBILITIES:
// ✓ Enforce business rules
// ✓ Validate data
// ✓ Handle business workflows
// ✓ Call DAL repositories
// ✓ Log business events
// ✗ NO HTTP concerns
// ✗ NO direct database access
```

### **DAL - Repositories (Data Access Layer)**
```csharp
// RESPONSIBILITIES:
// ✓ Execute database queries (ADO.NET)
// ✓ Map data to models
// ✓ Handle transactions
// ✓ Parameterized queries
// ✗ NO business logic
// ✗ NO HTTP concerns
```

## 📐 Communication Rules

```
✅ ALLOWED:
Controller → BLL Service → DAL Repository → Database

❌ NOT ALLOWED:
Controller → DAL Repository (skipping BLL)
BLL Service → Database (skipping DAL)
DAL Repository → BLL Service (reverse flow)
```

## 🔧 Dependency Injection Pattern

```csharp
// 1. Define Interface
public interface IUserService { }

// 2. Implement Interface
public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    
    public UserService(IUserRepository repository)
    {
        _repository = repository; // Injected
    }
}

// 3. Register in Program.cs
builder.Services.AddScoped<IUserService, UserService>();

// 4. Use in Controller
public class UserController : ControllerBase
{
    private readonly IUserService _service;
    
    public UserController(IUserService service)
    {
        _service = service; // Injected
    }
}
```

## 📋 Adding New Entity Checklist

- [ ] Create Model class in `Models/`
- [ ] Create interface in `DAL/Interfaces/I{Entity}Repository.cs`
- [ ] Implement repository in `DAL/Repositories/{Entity}Repository.cs`
- [ ] Create interface in `BLL/Interfaces/I{Entity}Service.cs`
- [ ] Implement service in `BLL/Services/{Entity}Service.cs`
- [ ] Create controller in `Controllers/{Entity}Controller.cs`
- [ ] Register DI in `Program.cs`:
  ```csharp
  builder.Services.AddScoped<I{Entity}Repository, {Entity}Repository>();
  builder.Services.AddScoped<I{Entity}Service, {Entity}Service>();
  ```
- [ ] Create database table with SQL script
- [ ] Test all endpoints in Swagger

## 💻 ADO.NET Quick Reference

### **Basic Query Pattern**
```csharp
using (var connection = new SqlConnection(_connectionString))
{
    var command = new SqlCommand("SELECT * FROM Table WHERE Id = @Id", connection);
    command.Parameters.AddWithValue("@Id", id);
    
    await connection.OpenAsync();
    
    using (var reader = await command.ExecuteReaderAsync())
    {
        while (await reader.ReadAsync())
        {
            // Map data
        }
    }
}
```

### **Insert with Return ID**
```csharp
var command = new SqlCommand(
    @"INSERT INTO Table (Column) VALUES (@Value);
      SELECT CAST(SCOPE_IDENTITY() as int);", connection);
      
var id = Convert.ToInt32(await command.ExecuteScalarAsync());
```

### **Update/Delete**
```csharp
var command = new SqlCommand("UPDATE Table SET Column = @Value WHERE Id = @Id", connection);
var rowsAffected = await command.ExecuteNonQueryAsync();
return rowsAffected > 0;
```

## 🎨 HTTP Status Code Guide

| Code | Meaning | When to Use |
|------|---------|-------------|
| 200 OK | Success | GET operations |
| 201 Created | Resource created | POST operations |
| 204 No Content | Success, no data | PUT/DELETE operations |
| 400 Bad Request | Invalid input | Validation errors |
| 404 Not Found | Resource missing | Entity not found |
| 409 Conflict | Business rule violated | Duplicate data |
| 500 Internal Server Error | Server error | Unexpected exceptions |

## 🔐 Security Best Practices

```csharp
// ✅ GOOD: Parameterized query
command.Parameters.AddWithValue("@Email", email);

// ❌ BAD: String concatenation (SQL Injection risk!)
var query = "SELECT * FROM Users WHERE Email = '" + email + "'";

// ✅ GOOD: Validate in BLL
if (string.IsNullOrWhiteSpace(user.Email))
    throw new ArgumentException("Email is required");

// ✅ GOOD: Try-catch with logging
try {
    // operation
} catch (Exception ex) {
    _logger.LogError(ex, "Error message");
    throw;
}
```

## 📊 Common Patterns

### **Soft Delete Pattern**
```csharp
// Don't actually delete, mark as inactive
user.IsActive = false;
await _repository.UpdateUserAsync(user);
```

### **Check Existence Before Operation**
```csharp
var existing = await _repository.GetByIdAsync(id);
if (existing == null)
    throw new InvalidOperationException("Not found");
```

### **Validate Uniqueness**
```csharp
var duplicate = await _repository.GetByEmailAsync(email);
if (duplicate != null && duplicate.Id != currentId)
    throw new InvalidOperationException("Email already exists");
```

## 🧪 Testing Each Layer

### **Controller Test**
```http
POST /api/User
Content-Type: application/json

{
  "username": "test",
  "email": "test@example.com",
  "fullName": "Test User"
}
```

### **Service Test** (Unit Test)
```csharp
// Mock repository
var mockRepo = new Mock<IUserRepository>();
var service = new UserService(mockRepo.Object, logger);

// Test business logic
await service.CreateUserAsync(user);
```

### **Repository Test** (Integration Test)
```csharp
// Requires actual database
var repo = new UserRepository(configuration);
var user = await repo.GetByIdAsync(1);
Assert.NotNull(user);
```

## 🚀 Performance Tips

```csharp
// ✅ Use async/await
public async Task<User> GetUserAsync(int id)

// ✅ Dispose resources properly
using (var connection = new SqlConnection(...))

// ✅ Use indexes on frequently queried columns
CREATE INDEX IX_Users_Email ON Users(Email);

// ✅ Return only needed columns
SELECT Id, Username, Email FROM Users

// ✅ Use connection pooling (automatic with SqlConnection)
```

## 📝 Naming Conventions

| Type | Convention | Example |
|------|-----------|---------|
| Model | PascalCase, singular | `User`, `Ticket` |
| Interface | I + PascalCase | `IUserService`, `IUserRepository` |
| Service | PascalCase + Service | `UserService`, `TicketService` |
| Repository | PascalCase + Repository | `UserRepository` |
| Controller | PascalCase + Controller | `UserController` |
| Method | PascalCase + Async | `GetUserAsync`, `CreateUserAsync` |
| Private field | _camelCase | `_userRepository`, `_logger` |

---

**Keep this file handy as a quick reference while developing!** 🚀
