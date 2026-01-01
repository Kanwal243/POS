# EyeHospitalPOS JWT Authentication API Documentation

## Overview

This document provides comprehensive information about the JWT-based Authentication and Role-Based Authorization API for the EyeHospitalPOS system.

## Base URL
```
https://localhost:5001/api
```

## Authentication

All protected endpoints require a valid JWT token in the Authorization header:
```
Authorization: Bearer {your_jwt_token}
```

---

## API Endpoints

### 1. Authentication Endpoints

#### POST `/api/auth/register`
Register a new user account.

**Request Body:**
```json
{
  "userName": "john.doe",
  "email": "john.doe@example.com",
  "password": "SecurePassword123",
  "confirmPassword": "SecurePassword123",
  "roleId": "6"  // Optional, defaults to General User
}
```

**Response (Success - 200 OK):**
```json
{
  "success": true,
  "message": "User registered successfully",
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "abc123def456...",
  "tokenExpiration": "2025-12-28T01:00:00Z",
  "user": {
    "id": "user-guid-here",
    "userName": "john.doe",
    "email": "john.doe@example.com",
    "roleId": "6",
    "roleName": "General User",
    "isActive": true,
    "createdDate": "2025-12-27T20:00:00Z"
  }
}
```

#### POST `/api/auth/login`
Authenticate user and receive JWT tokens.

**Request Body:**
```json
{
  "userName": "admin",
  "password": "Admin@123"
}
```

**Response (Success - 200 OK):**
```json
{
  "success": true,
  "message": "Login successful",
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "xyz789abc456...",
  "tokenExpiration": "2025-12-27T21:00:00Z",
  "user": {
    "id": "1",
    "userName": "admin",
    "email": "admin@eyehospital.com",
    "roleId": "1",
    "roleName": "Administrator",
    "isActive": true,
    "createdDate": "2025-12-22T00:00:00Z"
  }
}
```

**Response (Error - 401 Unauthorized):**
```json
{
  "success": false,
  "message": "Invalid username or password"
}
```

#### POST `/api/auth/refresh-token`
Refresh expired access token using refresh token.

**Request Body:**
```json
{
  "accessToken": "expired_access_token_here",
  "refreshToken": "valid_refresh_token_here"
}
```

**Response (Success - 200 OK):**
```json
{
  "success": true,
  "message": "Token refreshed successfully",
  "accessToken": "new_access_token_here",
  "refreshToken": "new_refresh_token_here",
  "tokenExpiration": "2025-12-27T22:00:00Z",
  "user": {
    "id": "1",
    "userName": "admin",
    "email": "admin@eyehospital.com",
    "roleId": "1",
    "roleName": "Administrator",
    "isActive": true,
    "createdDate": "2025-12-22T00:00:00Z"
  }
}
```

#### POST `/api/auth/revoke-token`
Revoke all refresh tokens for the current user (Logout).

**Headers:**
```
Authorization: Bearer {your_jwt_token}
```

**Response (Success - 200 OK):**
```json
{
  "message": "Tokens revoked successfully"
}
```

#### POST `/api/auth/change-password`
Change password for authenticated user.

**Headers:**
```
Authorization: Bearer {your_jwt_token}
```

**Request Body:**
```json
{
  "currentPassword": "OldPassword123",
  "newPassword": "NewPassword456",
  "confirmPassword": "NewPassword456"
}
```

**Response (Success - 200 OK):**
```json
{
  "message": "Password changed successfully"
}
```

#### GET `/api/auth/me`
Get current authenticated user information (Protected Route Example).

**Headers:**
```
Authorization: Bearer {your_jwt_token}
```

**Response (Success - 200 OK):**
```json
{
  "userId": "1",
  "userName": "admin",
  "email": "admin@eyehospital.com",
  "role": "Administrator",
  "claims": [
    { "type": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "value": "1" },
    { "type": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", "value": "admin" },
    { "type": "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "value": "Administrator" }
  ]
}
```

---

### 2. Role Management Endpoints (Admin Only)

#### GET `/api/roles`
Get all roles in the system.

**Headers:**
```
Authorization: Bearer {admin_jwt_token}
```

**Authorization:** Administrator role required

**Response (Success - 200 OK):**
```json
[
  {
    "id": "1",
    "name": "Administrator",
    "description": "Full system access with all permissions",
    "isAdministrative": true
  },
  {
    "id": "2",
    "name": "Inventory Admin",
    "description": "Full inventory management access",
    "isAdministrative": false
  }
]
```

#### GET `/api/roles/{id}`
Get role details by ID.

**Headers:**
```
Authorization: Bearer {admin_jwt_token}
```

**Authorization:** Administrator role required

**Response (Success - 200 OK):**
```json
{
  "id": "1",
  "name": "Administrator",
  "description": "Full system access with all permissions",
  "isAdministrative": true
}
```

#### POST `/api/roles`
Create a new role.

**Headers:**
```
Authorization: Bearer {admin_jwt_token}
```

**Authorization:** Administrator role required

**Request Body:**
```json
{
  "name": "Store Manager",
  "description": "Manages store operations",
  "isAdministrative": false,
  "canEditModel": true
}
```

**Response (Success - 201 Created):**
```json
{
  "id": "7",
  "name": "Store Manager",
  "description": "Manages store operations",
  "isAdministrative": false
}
```

#### PUT `/api/roles/{id}`
Update an existing role.

**Headers:**
```
Authorization: Bearer {admin_jwt_token}
```

**Authorization:** Administrator role required

**Request Body:**
```json
{
  "name": "Updated Role Name",
  "description": "Updated description",
  "isAdministrative": false,
  "canEditModel": true
}
```

**Response (Success - 200 OK):**
```json
{
  "id": "7",
  "name": "Updated Role Name",
  "description": "Updated description",
  "isAdministrative": false
}
```

#### DELETE `/api/roles/{id}`
Delete a role.

**Headers:**
```
Authorization: Bearer {admin_jwt_token}
```

**Authorization:** Administrator role required

**Response (Success - 200 OK):**
```json
{
  "message": "Role deleted successfully"
}
```

#### POST `/api/roles/assign`
Assign a role to a user.

**Headers:**
```
Authorization: Bearer {admin_jwt_token}
```

**Authorization:** Administrator role required

**Request Body:**
```json
{
  "userId": "user-guid-here",
  "roleId": "2"
}
```

**Response (Success - 200 OK):**
```json
{
  "message": "Role assigned successfully",
  "userId": "user-guid-here",
  "userName": "john.doe",
  "roleId": "2",
  "roleName": "Inventory Admin"
}
```

---

### 3. User Management Endpoints

#### GET `/api/users`
Get all users (Admin only).

**Headers:**
```
Authorization: Bearer {admin_jwt_token}
```

**Authorization:** Administrator role required

**Response (Success - 200 OK):**
```json
[
  {
    "id": "1",
    "userName": "admin",
    "email": "admin@eyehospital.com",
    "roleId": "1",
    "roleName": "Administrator",
    "isActive": true,
    "createdDate": "2025-12-22T00:00:00Z"
  }
]
```

#### GET `/api/users/{id}`
Get user by ID (Admin or self).

**Headers:**
```
Authorization: Bearer {your_jwt_token}
```

**Authorization:** User can view their own profile, Administrators can view any profile

**Response (Success - 200 OK):**
```json
{
  "id": "1",
  "userName": "admin",
  "email": "admin@eyehospital.com",
  "roleId": "1",
  "roleName": "Administrator",
  "isActive": true,
  "createdDate": "2025-12-22T00:00:00Z"
}
```

#### PUT `/api/users/{id}/activate`
Activate a deactivated user (Admin only).

**Headers:**
```
Authorization: Bearer {admin_jwt_token}
```

**Authorization:** Administrator role required

**Response (Success - 200 OK):**
```json
{
  "message": "User activated successfully"
}
```

#### PUT `/api/users/{id}/deactivate`
Deactivate a user (Admin only).

**Headers:**
```
Authorization: Bearer {admin_jwt_token}
```

**Authorization:** Administrator role required

**Response (Success - 200 OK):**
```json
{
  "message": "User deactivated successfully"
}
```

#### DELETE `/api/users/{id}`
Delete a user (Admin only).

**Headers:**
```
Authorization: Bearer {admin_jwt_token}
```

**Authorization:** Administrator role required

**Response (Success - 200 OK):**
```json
{
  "message": "User deleted successfully"
}
```

---

## Available Roles

| ID | Role Name | Description | Capabilities |
|----|-----------|-------------|--------------|
| 1 | Administrator | Full system access with all permissions | Full Access |
| 2 | Inventory Admin | Full inventory management access | Inventory Management |
| 3 | Inventory Clerk | Inventory receiving and basic operations | Inventory Operations |
| 4 | Inventory Reports Viewer | View inventory reports only | Read-Only Inventory |
| 5 | Sales Clerk | Process sales and manage customers | Sales Operations |
| 6 | General User | Basic system access | Limited Access |

---

## JWT Token Details

### Access Token
- **Expiration:** 60 minutes
- **Includes Claims:**
  - User ID
  - Username
  - Email
  - Role Name
  - Role ID
  - IsAdministrative flag

### Refresh Token
- **Expiration:** 7 days
- **Usage:** Used to obtain new access tokens without re-authentication
- **Security:** Stored in database, can be revoked

---

## Error Responses

### 400 Bad Request
```json
{
  "success": false,
  "message": "Validation error message here"
}
```

### 401 Unauthorized
```json
{
  "success": false,
  "message": "Invalid credentials or token"
}
```

### 403 Forbidden
```json
{
  "message": "You do not have permission to access this resource"
}
```

### 404 Not Found
```json
{
  "message": "Resource not found"
}
```

---

## Example Usage

### Using cURL

**Login:**
```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"userName":"admin","password":"Admin@123"}'
```

**Get Current User:**
```bash
curl -X GET https://localhost:5001/api/auth/me \
  -H "Authorization: Bearer your_jwt_token_here"
```

**Create Role (Admin Only):**
```bash
curl -X POST https://localhost:5001/api/roles \
  -H "Authorization: Bearer admin_jwt_token_here" \
  -H "Content-Type: application/json" \
  -d '{"name":"New Role","description":"Description here","isAdministrative":false,"canEditModel":true}'
```

### Using JavaScript/Fetch

```javascript
// Login
const login = async () => {
  const response = await fetch('https://localhost:5001/api/auth/login', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      userName: 'admin',
      password: 'Admin@123'
    })
  });
  
  const data = await response.json();
  
  if (data.success) {
    localStorage.setItem('accessToken', data.accessToken);
    localStorage.setItem('refreshToken', data.refreshToken);
  }
};

// Make authenticated request
const getMe = async () => {
  const token = localStorage.getItem('accessToken');
  
  const response = await fetch('https://localhost:5001/api/auth/me', {
    headers: {
      'Authorization': `Bearer ${token}`
    }
  });
  
  const data = await response.json();
  console.log(data);
};
```

---

## Security Best Practices

1. **Always use HTTPS** in production
2. **Store tokens securely** (HttpOnly cookies or secure storage)
3. **Implement token rotation** using refresh tokens
4. **Validate all user input** on both client and server
5. **Use strong passwords** (minimum 6 characters, alphanumeric recommended)
6. **Revoke tokens** on logout
7. **Monitor failed login attempts**
8. **Implement rate limiting** to prevent brute force attacks

---

## Swagger/OpenAPI Documentation

This API includes Swagger documentation accessible at:
```
https://localhost:5001/swagger
```

You can test all endpoints directly from the Swagger UI in development mode.

---

## Support

For issues or questions, please contact the development team.
