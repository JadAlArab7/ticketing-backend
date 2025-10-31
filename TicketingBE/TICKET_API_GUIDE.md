# Ticket API Documentation

## Overview
This document describes the full CRUD implementation for the Ticket module, including related entities (assignees and files) and list operations with sorting capabilities.

## Architecture
The Ticket module follows the three-tier architecture pattern:
- **Controller Layer**: `TicketController` - HTTP endpoints
- **BLL Layer**: `TicketService` - Business logic and JWT authentication
- **DAL Layer**: `TicketRepository`, `TicketAssigneeRepository`, `TicketFileRepository` - Data access

## Database Schema

### Tickets Table
```sql
CREATE TABLE tck.tickets(
    id UUID PRIMARY KEY,
    ticket_type_id UUID NOT NULL REFERENCES tck.ticket_types(id),
    subject TEXT check (subject <> ''),
    description TEXT check (description <> ''),
    alert_buffer TIMESTAMP,
    deadline TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ticket_status UUID REFERENCES tck.ticket_status(id),
    created_by UUID NOT NULL REFERENCES tck.departments(id)
);
```

### Ticket Assignees Table
```sql
CREATE TABLE tck.ticket_assignees(
    ticket_id UUID NOT NULL REFERENCES tck.tickets(id),
    department_id UUID NOT NULL REFERENCES tck.departments(id),
    ticket_assignee_type UUID NOT NULL REFERENCES tck.ticket_assignee_types(id),
    PRIMARY KEY (ticket_id, department_id)
);
```

### Ticket Files Table
```sql
CREATE TABLE tck.ticket_files(
    id UUID PRIMARY KEY,
    file_name TEXT NOT NULL,
    content_type TEXT,
    file_data BYTEA NOT NULL,
    ticket_id UUID NOT NULL REFERENCES tck.tickets(id),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by UUID NOT NULL REFERENCES tck.departments(id)
);
```

## API Endpoints

All endpoints require JWT authentication via `[Authorize]` attribute.

### 1. Get All Tickets (with sorting)
```http
GET /api/Ticket?sortBy={column}&order={direction}
```

**Query Parameters:**
- `sortBy` (optional): Column to sort by
  - `title` or `subject`: Sort by ticket subject
  - `createdbyuser` or `createdby`: Sort by creator department name
  - `type` or `tickettype`: Sort by ticket type name
  - `deadline`: Sort by deadline
  - `createdat`: Sort by creation date
- `order` (optional): Sort order (`ASC` or `DESC`, default: `DESC`)

**Response:** `200 OK`
```json
[
  {
    "id": "guid-string",
    "subject": "Ticket subject",
    "ticketTypeName": "Type name",
    "ticketStatusName": "Status name",
    "createdByDepartmentName": "Department name",
    "deadline": "2024-01-15T10:00:00Z",
    "createdAt": "2024-01-10T08:30:00Z",
    "assignedToDepartments": ["Dept 1", "Dept 2"]
  }
]
```

### 2. Get Ticket By ID (with full details)
```http
GET /api/Ticket/{id}
```

**Response:** `200 OK`
```json
{
  "id": "guid-string",
  "ticketTypeId": "guid-string",
  "ticketTypeName": "Type name",
  "subject": "Ticket subject",
  "description": "Detailed description",
  "alertBuffer": "2024-01-14T10:00:00Z",
  "deadline": "2024-01-15T10:00:00Z",
  "createdAt": "2024-01-10T08:30:00Z",
  "ticketStatus": "guid-string",
  "ticketStatusName": "Status name",
  "createdBy": "guid-string",
  "createdByDepartmentName": "Department name",
  "assignees": [
    {
      "ticketId": "guid-string",
      "departmentId": "guid-string",
      "departmentName": "Department name",
      "ticketAssigneeType": "guid-string",
      "ticketAssigneeTypeName": "Type name"
    }
  ],
  "files": [
    {
      "id": "guid-string",
      "fileName": "document.pdf",
      "contentType": "application/pdf",
      "ticketId": "guid-string",
      "createdAt": "2024-01-10T08:35:00Z",
      "createdBy": "guid-string"
    }
  ]
}
```

**Response:** `404 Not Found`
```json
{
  "message": "Ticket with ID {id} not found"
}
```

### 3. Create Ticket
```http
POST /api/Ticket
```

**Request Body:**
```json
{
  "ticketTypeId": "guid-string",
  "subject": "Ticket subject",
  "description": "Detailed description",
  "alertBuffer": "2024-01-14T10:00:00Z",
  "deadline": "2024-01-15T10:00:00Z",
  "ticketStatus": "guid-string",
  "assignees": [
    {
      "departmentId": "guid-string",
      "ticketAssigneeType": "guid-string"
    }
  ],
  "files": [
    {
      "fileName": "document.pdf",
      "contentType": "application/pdf",
      "fileData": "base64-encoded-bytes"
    }
  ]
}
```

**Notes:**
- `createdBy` is automatically extracted from JWT token (userId claim = departmentId)
- All operations (ticket, assignees, files) are executed in a database transaction
- Returns `201 Created` with location header pointing to the new ticket

**Response:** `201 Created`
```json
{
  "id": "new-guid-string"
}
```

### 4. Update Ticket
```http
PUT /api/Ticket/{id}
```

**Request Body:**
```json
{
  "id": "guid-string",
  "ticketTypeId": "guid-string",
  "subject": "Updated subject",
  "description": "Updated description",
  "alertBuffer": "2024-01-14T10:00:00Z",
  "deadline": "2024-01-15T10:00:00Z",
  "ticketStatus": "guid-string",
  "assignees": [
    {
      "departmentId": "guid-string",
      "ticketAssigneeType": "guid-string"
    }
  ],
  "files": []
}
```

**Notes:**
- The `id` in the URL must match the `id` in the request body
- Assignees are replaced (old ones deleted, new ones added) in a transaction
- Files are NOT updated through this endpoint (use separate file management endpoints if needed)
- Returns `404` if ticket not found

**Response:** `200 OK`
```json
{
  "message": "Ticket updated successfully"
}
```

### 5. Delete Ticket
```http
DELETE /api/Ticket/{id}
```

**Notes:**
- Deletes the ticket and all related data (assignees and files) in a transaction
- Returns `404` if ticket not found

**Response:** `200 OK`
```json
{
  "message": "Ticket deleted successfully"
}
```

## Transaction Support

The following operations use database transactions to ensure data consistency:

1. **Create Ticket**: Inserts ticket → inserts assignees → inserts files (all or nothing)
2. **Update Ticket**: Updates ticket → deletes old assignees → inserts new assignees (all or nothing)
3. **Delete Ticket**: Deletes assignees → deletes files → deletes ticket (all or nothing)

If any operation fails, the entire transaction is rolled back.

## File Handling

### File Storage
- Files are stored as **BYTEA** (binary data) in PostgreSQL
- File data should be sent as **base64-encoded strings** in JSON
- Maximum file size is limited by PostgreSQL and server configuration

### File Metadata
When listing tickets or retrieving ticket details, file metadata is returned without the binary data to reduce response size. File downloads would require a separate endpoint (not yet implemented).

## Authentication & Authorization

All endpoints require JWT authentication:
- Header: `Authorization: Bearer {token}`
- The `createdBy` field is automatically populated from the JWT token's `userId` claim
- Users can only create tickets for their own department (enforced by token validation)

## Error Handling

All endpoints return consistent error responses:

**400 Bad Request**: Invalid input data
```json
{
  "message": "ID mismatch"
}
```

**401 Unauthorized**: Missing or invalid JWT token
```json
{
  "message": "User ID not found in token"
}
```

**404 Not Found**: Resource not found
```json
{
  "message": "Ticket with ID {id} not found"
}
```

**500 Internal Server Error**: Server-side error
```json
{
  "message": "An error occurred while creating the ticket"
}
```

## Data Type Handling

### GUID ↔ String Conversion
- **Application Layer**: Uses `string` for all IDs (clean API contracts)
- **Database Layer**: Uses `UUID` (PostgreSQL native type)
- **Conversion Point**: At the repository boundary
  - Reading: `TypeHelper.GetGuidAsString(reader, "id")`
  - Writing: `SqlHelper.CreateParam("@id", Guid.Parse(stringId))`

### DateTime Handling
- All timestamps are stored in UTC
- Nullable DateTimes (`alertBuffer`, `deadline`) are properly handled
- `TypeHelper.GetNullableDateTime()` used for safe conversion

## Performance Considerations

1. **List Endpoint**: Loads assignees separately for each ticket (N+1 query pattern)
   - Could be optimized with a JOIN query if performance becomes an issue
   
2. **File Data**: Binary data is excluded from list queries to reduce payload size
   
3. **Sorting**: Dynamic ORDER BY clause built server-side (safe from SQL injection)

## Future Enhancements

Potential improvements not yet implemented:
1. **File Download Endpoint**: `GET /api/Ticket/{id}/files/{fileId}/download`
2. **Pagination**: Add skip/take parameters for large result sets
3. **Filtering**: Add query parameters to filter by status, type, assignee, etc.
4. **Bulk Operations**: Create/update multiple tickets in one request
5. **File Size Validation**: Enforce maximum file size limits
6. **File Type Validation**: Restrict allowed file types/extensions

## Dependencies

The Ticket module depends on:
- `ITicketRepository`, `ITicketAssigneeRepository`, `ITicketFileRepository` (DAL)
- `ITicketService` (BLL)
- `SqlHelper` (database operations)
- `TypeHelper` (type conversions)
- `IHttpContextAccessor` (JWT claims extraction)
- `ILogger<T>` (logging)

All dependencies are registered in `ServiceCollectionExtensions.cs`.
