# Visual Architecture Diagram

## 📊 Complete Three-Tier Architecture

```
╔════════════════════════════════════════════════════════════════════════════╗
║                          CLIENT / API CONSUMER                              ║
║                    (Postman, Frontend App, Mobile App)                      ║
╚════════════════════════════════════════════════════════════════════════════╝
                                    ↕ HTTP/HTTPS
╔════════════════════════════════════════════════════════════════════════════╗
║                       PRESENTATION LAYER (Controllers)                      ║
║  ┌──────────────────────────────────────────────────────────────────────┐  ║
║  │                        UserController.cs                              │  ║
║  │  ┌────────────────┐  ┌────────────────┐  ┌────────────────────────┐ │  ║
║  │  │ GetAllUsers()  │  │ CreateUser()   │  │ GetUserByEmail()       │ │  ║
║  │  │ [HttpGet]      │  │ [HttpPost]     │  │ [HttpGet("email/..."]  │ │  ║
║  │  └────────────────┘  └────────────────┘  └────────────────────────┘ │  ║
║  │  ┌────────────────┐  ┌────────────────┐  ┌────────────────────────┐ │  ║
║  │  │ GetUserById()  │  │ UpdateUser()   │  │ DeactivateUser()       │ │  ║
║  │  │ [HttpGet("{id}")]│ [HttpPut]       │  │ [HttpDelete(".../de..")]│ │  ║
║  │  └────────────────┘  └────────────────┘  └────────────────────────┘ │  ║
║  │                                                                        │  ║
║  │  Responsibilities:                                                     │  ║
║  │  • Handle HTTP requests/responses                                     │  ║
║  │  • Route parameters validation                                        │  ║
║  │  • Return appropriate status codes                                    │  ║
║  │  • Exception handling & error messages                                │  ║
║  └──────────────────────────────────────────────────────────────────────┘  ║
╚════════════════════════════════════════════════════════════════════════════╝
                                    ↕ Interface
                         IUserService (Dependency Injection)
╔════════════════════════════════════════════════════════════════════════════╗
║                    BUSINESS LOGIC LAYER (BLL/Services)                     ║
║  ┌──────────────────────────────────────────────────────────────────────┐  ║
║  │                    IUserService / UserService.cs                      │  ║
║  │                                                                        │  ║
║  │  Business Methods:                                                     │  ║
║  │  • GetAllActiveUsersAsync()     → Filters active users                │  ║
║  │  • GetUserByIdAsync()           → Validates ID > 0                    │  ║
║  │  • GetUserByEmailAsync()        → Validates email format              │  ║
║  │  • CreateUserAsync()            → Email uniqueness check              │  ║
║  │  • UpdateUserAsync()            → Duplicate validation                │  ║
║  │  • DeactivateUserAsync()        → Soft delete implementation          │  ║
║  │  • DeleteUserAsync()            → Hard delete with logging            │  ║
║  │                                                                        │  ║
║  │  Business Rules:                                                       │  ║
║  │  ✓ Email format validation (using MailAddress)                        │  ║
║  │  ✓ Required field validation (Username, Email, FullName)             │  ║
║  │  ✓ Duplicate email prevention                                         │  ║
║  │  ✓ Set default values (CreatedAt, IsActive)                           │  ║
║  │  ✓ Comprehensive logging                                              │  ║
║  │  ✓ Exception handling & meaningful error messages                     │  ║
║  └──────────────────────────────────────────────────────────────────────┘  ║
╚════════════════════════════════════════════════════════════════════════════╝
                                    ↕ Interface
                       IUserRepository (Dependency Injection)
╔════════════════════════════════════════════════════════════════════════════╗
║                   DATA ACCESS LAYER (DAL/Repositories)                     ║
║  ┌──────────────────────────────────────────────────────────────────────┐  ║
║  │              IUserRepository / UserRepository.cs                      │  ║
║  │                         (Using ADO.NET)                                │  ║
║  │                                                                        │  ║
║  │  Database Operations:                                                  │  ║
║  │  • GetAllUsersAsync()        → SELECT all users                       │  ║
║  │  • GetUserByIdAsync()        → SELECT WHERE Id = @Id                  │  ║
║  │  • GetUserByEmailAsync()     → SELECT WHERE Email = @Email            │  ║
║  │  • CreateUserAsync()         → INSERT + return new ID                 │  ║
║  │  • UpdateUserAsync()         → UPDATE WHERE Id = @Id                  │  ║
║  │  • DeleteUserAsync()         → DELETE WHERE Id = @Id                  │  ║
║  │                                                                        │  ║
║  │  ADO.NET Components:                                                   │  ║
║  │  ┌────────────────┐  ┌────────────────┐  ┌────────────────────────┐ │  ║
║  │  │ SqlConnection  │  │  SqlCommand    │  │   SqlDataReader        │ │  ║
║  │  │ • Connection   │  │  • Query       │  │   • Read results       │ │  ║
║  │  │   pooling      │  │  • Parameters  │  │   • Map to objects     │ │  ║
║  │  │ • Open/Close   │  │  • Execute     │  │   • Async iteration    │ │  ║
║  │  └────────────────┘  └────────────────┘  └────────────────────────┘ │  ║
║  │                                                                        │  ║
║  │  Best Practices:                                                       │  ║
║  │  ✓ Parameterized queries (SQL injection prevention)                   │  ║
║  │  ✓ Using statements (proper resource disposal)                        │  ║
║  │  ✓ Async/await pattern                                                │  ║
║  │  ✓ MapReaderToUser() helper method                                    │  ║
║  └──────────────────────────────────────────────────────────────────────┘  ║
╚════════════════════════════════════════════════════════════════════════════╝
                                    ↕ ADO.NET
╔════════════════════════════════════════════════════════════════════════════╗
║                            SQL SERVER DATABASE                             ║
║  ┌──────────────────────────────────────────────────────────────────────┐  ║
║  │                            TicketingDB                                 │  ║
║  │                                                                        │  ║
║  │  Tables:                                                               │  ║
║  │  ┌─────────────────────────────────────────────────────────────────┐ │  ║
║  │  │ Users                                                            │ │  ║
║  │  │  • Id (INT, PRIMARY KEY, IDENTITY)                               │ │  ║
║  │  │  • Username (NVARCHAR(100), NOT NULL)                            │ │  ║
║  │  │  • Email (NVARCHAR(255), NOT NULL, UNIQUE)                       │ │  ║
║  │  │  • FullName (NVARCHAR(200), NOT NULL)                            │ │  ║
║  │  │  • CreatedAt (DATETIME2, NOT NULL)                               │ │  ║
║  │  │  • IsActive (BIT, NOT NULL, DEFAULT 1)                           │ │  ║
║  │  └─────────────────────────────────────────────────────────────────┘ │  ║
║  │                                                                        │  ║
║  │  Indexes:                                                              │  ║
║  │  • IX_Users_Email (for fast email lookups)                            │  ║
║  │  • IX_Users_IsActive (for filtering active users)                     │  ║
║  └──────────────────────────────────────────────────────────────────────┘  ║
╚════════════════════════════════════════════════════════════════════════════╝
```

## 🔄 Request Flow Example: Creating a User

```
1. CLIENT sends POST request:
   POST /api/User
   Body: { "username": "john", "email": "john@example.com", "fullName": "John Doe" }
   
   ↓

2. CONTROLLER (UserController.cs)
   • CreateUser() method receives request
   • Validates HTTP payload
   • Calls: await _userService.CreateUserAsync(user)
   
   ↓

3. BLL (UserService.cs)
   • Validates: Username, Email, FullName not empty
   • Validates: Email format is correct
   • Checks: Email doesn't exist (calls _userRepository.GetUserByEmailAsync)
   • Sets: CreatedAt = UtcNow, IsActive = true
   • Calls: await _userRepository.CreateUserAsync(user)
   
   ↓

4. DAL (UserRepository.cs)
   • Creates SqlConnection with connection string
   • Prepares INSERT query with parameters
   • Executes: command.ExecuteScalarAsync()
   • Returns: New user ID
   
   ↓

5. DATABASE (SQL Server)
   • Inserts new record into Users table
   • Generates Id using IDENTITY
   • Returns new Id via SCOPE_IDENTITY()
   
   ↓

6. RESPONSE flows back:
   DAL returns Id → BLL returns Id → Controller retrieves user → 
   Returns 201 Created with user object and Location header
```

## 📦 Dependency Injection Configuration

```
┌─────────────────────────────────────────────────────────────────────┐
│                         Program.cs                                   │
│                                                                      │
│  builder.Services.AddScoped<IUserRepository, UserRepository>();     │
│  builder.Services.AddScoped<IUserService, UserService>();           │
│                                                                      │
│  ┌────────────────────────────────────────────────────────────────┐ │
│  │  When UserController needs IUserService:                       │ │
│  │  1. DI Container creates instance of UserService              │ │
│  │  2. UserService constructor needs IUserRepository             │ │
│  │  3. DI Container creates instance of UserRepository           │ │
│  │  4. UserRepository constructor needs IConfiguration           │ │
│  │  5. DI Container provides IConfiguration (built-in)           │ │
│  │  6. All dependencies resolved, UserController created         │ │
│  └────────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────────┘

Lifetime: AddScoped
• One instance per HTTP request
• Disposed after request completes
• Ideal for database operations
```

## 🛡️ Error Handling Flow

```
Exception occurs in DAL
        ↓
    Caught in BLL
        ↓
    Logged by BLL
        ↓
    Re-thrown or converted to business exception
        ↓
    Caught in Controller
        ↓
    Logged by Controller
        ↓
    Converted to appropriate HTTP status code
        ↓
    Returned to client with user-friendly message
```

## 📁 Files Created

### Core Architecture
- `Models/User.cs` - Entity model
- `DAL/Interfaces/IUserRepository.cs` - Repository contract
- `DAL/Repositories/UserRepository.cs` - ADO.NET implementation
- `BLL/Interfaces/IUserService.cs` - Service contract
- `BLL/Services/UserService.cs` - Business logic
- `Controllers/UserController.cs` - API endpoints
- `Program.cs` - DI configuration

### Configuration
- `appsettings.json` - Production config
- `appsettings.Development.json` - Development config
- `TicketingBE.csproj` - Project file with dependencies

### Documentation
- `ARCHITECTURE.md` - Complete architecture documentation
- `SETUP_COMPLETE.md` - Quick start guide
- `QUICK_REFERENCE.md` - Developer reference
- `VISUAL_DIAGRAM.md` - This file
- `DatabaseSetup.sql` - Database creation script

## 🎯 Key Principles Demonstrated

1. **Separation of Concerns**: Each layer has distinct responsibility
2. **Dependency Inversion**: Depend on abstractions (interfaces), not implementations
3. **Single Responsibility**: Each class has one reason to change
4. **Open/Closed**: Open for extension, closed for modification
5. **Interface Segregation**: Small, focused interfaces
6. **Don't Repeat Yourself**: Reusable components
7. **Async/Await**: Non-blocking operations
8. **Dependency Injection**: Loose coupling, testability

---

**This visual diagram provides a comprehensive overview of the complete architecture!** 🎨
