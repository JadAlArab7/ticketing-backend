# Login API Implementation Guide

## Overview
A complete JWT-based authentication system has been implemented in the three-tier architecture, integrating with your PostgreSQL database schema (`tck.users`, `tck.departments`, `tck.department_types`).

## Endpoint

### POST /api/User/login
Authenticates a user and returns a JWT token with user information.

**Request Body:**
```json
{
  "username": "string",
  "password": "string"
}
```

**Success Response (200 OK):**
```json
{
  "id": "uuid",
  "username": "string",
  "departmentName": "string",
  "departmentType": "string",
  "token": "jwt-token-string"
}
```

**Error Responses:**
- `400 Bad Request` - Invalid request format or missing fields
- `401 Unauthorized` - Invalid username or password
- `500 Internal Server Error` - Server error during authentication

## JWT Token Details

### Claims Included
- `sub` (Subject): User ID (UUID)
- `jti` (JWT ID): Unique token identifier
- `userId`: User ID
- `username`: Username
- `department`: Department name
- `departmentType`: Department type name

### Configuration
Token settings are in `appsettings.json`:
```json
{
  "JwtSettings": {
    "SecretKey": "YourSecretKeyMustBeAtLeast64CharactersLongForHS256Algorithm",
    "Issuer": "TicketingBackend",
    "Audience": "TicketingFrontend",
    "ExpirationMinutes": 60
  }
}
```

## Testing the API

### Using Swagger UI
1. Run the application
2. Navigate to `https://localhost:<port>/swagger`
3. Find the `POST /api/User/login` endpoint
4. Click "Try it out"
5. Enter test credentials:
```json
{
  "username": "testuser",
  "password": "testpassword"
}
```
6. Click "Execute"

### Using cURL
```bash
curl -X POST "https://localhost:5001/api/User/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"testpassword"}'
```

### Using Postman
1. Method: POST
2. URL: `https://localhost:5001/api/User/login`
3. Headers: `Content-Type: application/json`
4. Body (raw JSON):
```json
{
  "username": "testuser",
  "password": "testpassword"
}
```

## Using the JWT Token

After successful login, use the returned token for authenticated requests:

### Authorization Header
```
Authorization: Bearer <your-jwt-token>
```

### Example with cURL
```bash
curl -X GET "https://localhost:5001/api/SomeProtectedEndpoint" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

## Database Requirements

### Prerequisites
Ensure your PostgreSQL database has test data:

```sql
-- Example: Insert a test user (adjust based on your actual schema)
INSERT INTO tck.users (id, username, password, department_id)
VALUES (
  gen_random_uuid(), 
  'testuser', 
  'testpassword',  -- In production, use hashed passwords!
  '<existing-department-uuid>'
);
```

⚠️ **IMPORTANT**: The current implementation stores passwords in plain text. For production, you MUST implement password hashing (e.g., BCrypt, Argon2).

## Implementation Details

### Architecture Flow
```
Controller (UserController.Login)
    ↓
Business Logic (UserService.LoginAsync)
    ↓ [Validates input, generates JWT]
Data Access (UserRepository.AuthenticateUserAsync)
    ↓ [Queries database with JOINs]
PostgreSQL Database (tck.users + departments + department_types)
```

### Database Query
The authentication query joins three tables:
```sql
SELECT 
    u.id, 
    u.username, 
    u.password,
    u.department_id,
    d.name as department_name,
    dt.name as department_type
FROM tck.users u
INNER JOIN tck.departments d ON u.department_id = d.id
INNER JOIN tck.department_types dt ON d.department_type_id = dt.id
WHERE u.username = @username AND u.password = @password
```

## Files Modified/Created

### New Files
1. `Models/DTOs/LoginRequestDto.cs` - Login input model
2. `Models/DTOs/LoginResponseDto.cs` - Login response model

### Modified Files
1. `Program.cs` - Added JWT authentication middleware
2. `Models/User.cs` - Updated to match tck.users schema (UUID, departments)
3. `DAL/Interfaces/IUserRepository.cs` - Added AuthenticateUserAsync
4. `DAL/Repositories/UserRepository.cs` - Implemented authentication with JOINs
5. `BLL/Interfaces/IUserService.cs` - Added LoginAsync
6. `BLL/Services/UserService.cs` - Implemented JWT token generation
7. `Controllers/UserController.cs` - Added Login endpoint
8. `appsettings.json` / `appsettings.Development.json` - Added JWT settings

## Security Recommendations

### For Production Deployment

1. **Password Hashing**
   - Install: `dotnet add package BCrypt.Net-Next`
   - Hash passwords on registration: `BCrypt.Net.BCrypt.HashPassword(password)`
   - Verify on login: `BCrypt.Net.BCrypt.Verify(password, hashedPassword)`

2. **Secret Key Management**
   - Move `SecretKey` to environment variables or Azure Key Vault
   - Use a cryptographically secure random key (minimum 512 bits for HS256)
   - Generate: `openssl rand -base64 64`

3. **HTTPS Only**
   - Enforce HTTPS in production
   - Set secure cookie flags
   - Use HSTS headers

4. **Token Best Practices**
   - Implement token refresh mechanism
   - Add token blacklist for logout
   - Set appropriate expiration times
   - Include IP address validation

5. **Input Validation**
   - Add rate limiting for login attempts
   - Implement account lockout after failed attempts
   - Validate and sanitize all inputs

## Troubleshooting

### Common Issues

1. **"Invalid username or password"**
   - Verify user exists in database
   - Check username/password exact match (case-sensitive)
   - Ensure database connection is configured correctly

2. **"An error occurred during login"**
   - Check application logs for detailed error
   - Verify JWT settings in appsettings.json
   - Ensure SecretKey is at least 64 characters

3. **Token validation fails**
   - Verify Issuer and Audience match between generation and validation
   - Check token expiration
   - Ensure SecretKey is consistent

4. **Database connection errors**
   - Verify connection string in appsettings.json
   - Check PostgreSQL is running
   - Ensure database user has SELECT permissions on tck schema

## Next Steps

1. **Test the Login Endpoint**
   - Create test users in your database
   - Test with valid credentials
   - Test with invalid credentials
   - Verify JWT token is generated correctly

2. **Protect Other Endpoints**
   - Add `[Authorize]` attribute to controllers/actions that require authentication
   - Example:
   ```csharp
   [Authorize]
   [HttpGet]
   public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
   ```

3. **Implement Password Hashing**
   - Update CreateUserAsync to hash passwords
   - Update AuthenticateUserAsync to verify hashed passwords

4. **Add Token Refresh**
   - Create refresh token mechanism
   - Store refresh tokens in database
   - Implement /api/User/refresh endpoint

## Support

For issues or questions:
- Check application logs in the console or logging system
- Review the implementation files listed above
- Verify database schema matches expected structure (UUIDs, proper relationships)
