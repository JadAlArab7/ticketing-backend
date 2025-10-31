# SqlHelper Usage Examples

## Overview
The `SqlHelper` class provides a clean, reusable way to execute database operations using ADO.NET with SQL Server. It handles connection management, transactions, and parameterized queries.

## Basic Usage

### 1. Simple Query (Auto-managed connection)

```csharp
// Get all users
string query = "SELECT Id, Username, Email, FullName, CreatedAt, IsActive FROM Users";

var users = _sqlHelper.ExecuteReader<List<User>>(query, (dataReader) =>
{
    var userList = new List<User>();
    while (dataReader.Read())
    {
        userList.Add(new User
        {
            Id = TypeHelper.GetInt32(dataReader["Id"]),
            Username = TypeHelper.GetString(dataReader["Username"]),
            Email = TypeHelper.GetString(dataReader["Email"]),
            FullName = TypeHelper.GetString(dataReader["FullName"]),
            CreatedAt = TypeHelper.GetDateTime(dataReader["CreatedAt"]),
            IsActive = TypeHelper.GetBoolean(dataReader["IsActive"])
        });
    }
    return userList;
});
```

### 2. Query with Parameters

```csharp
// Get user by ID
string query = "SELECT Id, Username, Email FROM Users WHERE Id = @Id";

var user = _sqlHelper.ExecuteReader<User?>(query, (dataReader) =>
{
    User? user = null;
    if (dataReader.Read())
    {
        user = new User
        {
            Id = TypeHelper.GetInt32(dataReader["Id"]),
            Username = TypeHelper.GetString(dataReader["Username"]),
            Email = TypeHelper.GetString(dataReader["Email"])
        };
    }
    return user;
}, _sqlHelper.CreateParam("@Id", userId));
```

### 3. Query with Multiple Parameters

```csharp
// Get unread notifications count
string query = @"SELECT Count(*) Over() AS TotalCount 
                 FROM user_notifications un 
                 WHERE un.status_id = @unread_status_id 
                 AND un.user_receiver_id = @user_id";

int unreadCounter = _sqlHelper.ExecuteReader<int>(query, (dataReader) =>
{
    int unreadCount = 0;
    if (dataReader.Read())
    {
        unreadCount = TypeHelper.GetInt32(dataReader["TotalCount"]);
    }
    return unreadCount;
},
_sqlHelper.CreateParam("@unread_status_id", unreadStatusID),
_sqlHelper.CreateParam("@user_id", userID));
```

### 4. Insert/Update/Delete Operations

```csharp
// Insert a new user
string insertQuery = @"INSERT INTO Users (Username, Email, FullName, CreatedAt, IsActive) 
                      VALUES (@Username, @Email, @FullName, @CreatedAt, @IsActive)";

int rowsAffected = _sqlHelper.ExecuteData(insertQuery,
    _sqlHelper.CreateParam("@Username", user.Username),
    _sqlHelper.CreateParam("@Email", user.Email),
    _sqlHelper.CreateParam("@FullName", user.FullName),
    _sqlHelper.CreateParam("@CreatedAt", DateTime.UtcNow),
    _sqlHelper.CreateParam("@IsActive", true));

// Update user
string updateQuery = @"UPDATE Users 
                      SET Username = @Username, Email = @Email 
                      WHERE Id = @Id";

int rowsAffected = _sqlHelper.ExecuteData(updateQuery,
    _sqlHelper.CreateParam("@Id", userId),
    _sqlHelper.CreateParam("@Username", newUsername),
    _sqlHelper.CreateParam("@Email", newEmail));

// Delete user
string deleteQuery = "DELETE FROM Users WHERE Id = @Id";
int rowsAffected = _sqlHelper.ExecuteData(deleteQuery,
    _sqlHelper.CreateParam("@Id", userId));
```

### 5. Get Inserted ID

```csharp
string query = @"INSERT INTO Users (Username, Email, FullName, CreatedAt, IsActive) 
                VALUES (@Username, @Email, @FullName, @CreatedAt, @IsActive);
                SELECT CAST(SCOPE_IDENTITY() as int);";

int newUserId = _sqlHelper.ExecuteReader<int>(query, (dataReader) =>
{
    int userId = 0;
    if (dataReader.Read())
    {
        userId = TypeHelper.GetInt32(dataReader[0]);
    }
    return userId;
},
_sqlHelper.CreateParam("@Username", user.Username),
_sqlHelper.CreateParam("@Email", user.Email),
_sqlHelper.CreateParam("@FullName", user.FullName),
_sqlHelper.CreateParam("@CreatedAt", DateTime.UtcNow),
_sqlHelper.CreateParam("@IsActive", true));
```

## Advanced Usage - Manual Transaction Management

### 1. Multiple Operations in Single Transaction

```csharp
public async Task<bool> TransferTicketAsync(int ticketId, int fromUserId, int toUserId)
{
    IDbConnection connection = null;
    IDbTransaction transaction = null;
    
    try
    {
        // Create and open connection
        connection = new SqlConnection(_sqlHelper.GetConnectionString());
        _sqlHelper.OpenConnection(connection);
        
        // Begin transaction
        transaction = _sqlHelper.BeginTransaction(connection);
        
        // Operation 1: Update ticket assignment
        string updateTicketQuery = "UPDATE Tickets SET AssignedUserId = @ToUserId WHERE Id = @TicketId";
        int result1 = _sqlHelper.ExecuteData(connection, transaction, updateTicketQuery, false,
            _sqlHelper.CreateParam("@TicketId", ticketId),
            _sqlHelper.CreateParam("@ToUserId", toUserId));
        
        // Operation 2: Insert audit log
        string insertLogQuery = @"INSERT INTO AuditLogs (TicketId, FromUserId, ToUserId, Action, CreatedAt) 
                                 VALUES (@TicketId, @FromUserId, @ToUserId, 'Transfer', @CreatedAt)";
        int result2 = _sqlHelper.ExecuteData(connection, transaction, insertLogQuery, false,
            _sqlHelper.CreateParam("@TicketId", ticketId),
            _sqlHelper.CreateParam("@FromUserId", fromUserId),
            _sqlHelper.CreateParam("@ToUserId", toUserId),
            _sqlHelper.CreateParam("@CreatedAt", DateTime.UtcNow));
        
        // Operation 3: Update user statistics
        string updateStatsQuery = "UPDATE UserStats SET AssignedTickets = AssignedTickets + 1 WHERE UserId = @UserId";
        int result3 = _sqlHelper.ExecuteData(connection, transaction, updateStatsQuery, false,
            _sqlHelper.CreateParam("@UserId", toUserId));
        
        // If all operations succeed, commit
        transaction.Commit();
        return true;
    }
    catch (Exception ex)
    {
        // Rollback on error
        transaction?.Rollback();
        throw;
    }
    finally
    {
        // Clean up
        _sqlHelper.CloseConnection(connection);
    }
}
```

### 2. Using ExecuteReader with Manual Transaction

```csharp
public async Task<List<User>> GetUsersWithTransactionAsync()
{
    IDbConnection connection = null;
    IDbTransaction transaction = null;
    
    try
    {
        connection = new SqlConnection(_sqlHelper.GetConnectionString());
        _sqlHelper.OpenConnection(connection);
        transaction = _sqlHelper.BeginTransaction(connection);
        
        string query = "SELECT Id, Username, Email FROM Users WHERE IsActive = @IsActive";
        
        var users = _sqlHelper.ExecuteReader<List<User>>(
            connection, 
            transaction, 
            query, 
            (dataReader) =>
            {
                var userList = new List<User>();
                while (dataReader.Read())
                {
                    userList.Add(new User
                    {
                        Id = TypeHelper.GetInt32(dataReader["Id"]),
                        Username = TypeHelper.GetString(dataReader["Username"]),
                        Email = TypeHelper.GetString(dataReader["Email"])
                    });
                }
                return userList;
            },
            false, // Don't commit yet - we control it manually
            _sqlHelper.CreateParam("@IsActive", true)
        );
        
        // Do more operations if needed...
        
        transaction.Commit();
        return users;
    }
    catch (Exception ex)
    {
        transaction?.Rollback();
        throw;
    }
    finally
    {
        _sqlHelper.CloseConnection(connection);
    }
}
```

## TypeHelper Usage

The `TypeHelper` class provides safe type conversion from database values:

```csharp
// Get values safely (returns default if null)
int id = TypeHelper.GetInt32(dataReader["Id"]);                    // Returns 0 if null
long bigId = TypeHelper.GetInt64(dataReader["BigId"]);             // Returns 0 if null
string name = TypeHelper.GetString(dataReader["Name"]);            // Returns "" if null
bool isActive = TypeHelper.GetBoolean(dataReader["IsActive"]);     // Returns false if null
DateTime date = TypeHelper.GetDateTime(dataReader["CreatedAt"]);   // Returns MinValue if null
Guid guid = TypeHelper.GetGuid(dataReader["UniqueId"]);            // Returns Empty if null
decimal amount = TypeHelper.GetDecimal(dataReader["Amount"]);      // Returns 0 if null

// Get nullable values (returns null if null)
int? nullableId = TypeHelper.GetNullableInt32(dataReader["OptionalId"]);
string? nullableName = TypeHelper.GetNullableString(dataReader["MiddleName"]);
DateTime? nullableDate = TypeHelper.GetNullableDateTime(dataReader["CompletedAt"]);
Guid? nullableGuid = TypeHelper.GetNullableGuid(dataReader["OptionalGuid"]);
```

## Parameter Types

The `SqlHelper` provides convenient parameter creation methods:

```csharp
// Basic types
_sqlHelper.CreateParam("@IntValue", 123);                          // int
_sqlHelper.CreateParam("@LongValue", 123L);                        // long
_sqlHelper.CreateParam("@StringValue", "text");                    // string
_sqlHelper.CreateParam("@DateValue", DateTime.Now);                // DateTime
_sqlHelper.CreateParam("@BoolValue", true);                        // bool
_sqlHelper.CreateParam("@GuidValue", Guid.NewGuid());              // Guid
_sqlHelper.CreateParam("@ByteArray", new byte[] { 1, 2, 3 });      // byte[]
_sqlHelper.CreateParam("@DecimalValue", 123.45m);                  // decimal

// Custom SqlDbType with size
_sqlHelper.CreateParam("@CustomParam", value, SqlDbType.NVarChar, 255);

// Output parameters
_sqlHelper.CreateOutParam("@OutputId", SqlDbType.Int);
_sqlHelper.CreateOutParam("@OutputName", SqlDbType.NVarChar, 100);
```

## Best Practices

1. **Always use parameterized queries** to prevent SQL injection
2. **Use transactions** for multiple related operations
3. **Use TypeHelper** for safe type conversions
4. **Handle exceptions** appropriately and log errors
5. **Close connections** in finally blocks or use the auto-commit feature
6. **Wrap async operations** with Task.Run when using synchronous SqlHelper methods

## Repository Pattern Example

```csharp
public class UserRepository : IUserRepository
{
    private readonly SqlHelper _sqlHelper;

    public UserRepository(SqlHelper sqlHelper)
    {
        _sqlHelper = sqlHelper;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        string query = "SELECT * FROM Users WHERE Id = @Id";
        
        return await Task.Run(() =>
        {
            return _sqlHelper.ExecuteReader<User?>(query, (dataReader) =>
            {
                User? user = null;
                if (dataReader.Read())
                {
                    user = MapUser(dataReader);
                }
                return user;
            }, _sqlHelper.CreateParam("@Id", id));
        });
    }

    private User MapUser(DbDataReader reader)
    {
        return new User
        {
            Id = TypeHelper.GetInt32(reader["Id"]),
            Username = TypeHelper.GetString(reader["Username"]),
            Email = TypeHelper.GetString(reader["Email"]),
            FullName = TypeHelper.GetString(reader["FullName"]),
            CreatedAt = TypeHelper.GetDateTime(reader["CreatedAt"]),
            IsActive = TypeHelper.GetBoolean(reader["IsActive"])
        };
    }
}
```

---

**The SqlHelper provides a clean, reusable abstraction over ADO.NET while maintaining full control over connections, transactions, and query execution!**
