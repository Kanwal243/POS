# JWT API Quick Reference

## 🚀 Quick Start

### 1. Login (Get JWT Token)
```bash
POST /api/auth/login
{
  "userName": "admin",
  "password": "Admin@123"
}

# Response includes accessToken and refreshToken
```

### 2. Use Token in Requests
```bash
Authorization: Bearer YOUR_ACCESS_TOKEN_HERE
```

### 3. Refresh Expired Token
```bash
POST /api/auth/refresh-token
{
  "accessToken": "expired_token",
  "refreshToken": "valid_refresh_token"
}
```

---

## 🔑 Available Roles

| Role ID | Role Name | Description |
|---------|-----------|-------------|
| 1 | Administrator | Full access |
| 2 | Inventory Admin | Full inventory management |
| 3 | Inventory Clerk | Inventory operations |
| 4 | Inventory Reports Viewer | View only |
| 5 | Sales Clerk | Sales operations |
| 6 | General User | Basic access |

---

## 📡 Main Endpoints

### Authentication
- `POST /api/auth/register` - Register
- `POST /api/auth/login` - Login
- `POST /api/auth/refresh-token` - Refresh
- `POST /api/auth/revoke-token` 🔒 - Logout
- `POST /api/auth/change-password` 🔒 - Change pwd
- `GET /api/auth/me` 🔒 - Current user

### Roles (👑 Admin Only)
- `GET /api/roles` - List roles
- `POST /api/roles` - Create role
- `PUT /api/roles/{id}` - Update
- `DELETE /api/roles/{id}` - Delete
- `POST /api/roles/assign` - Assign to user

### Users
- `GET /api/users` 👑 - List all
- `GET /api/users/{id}` 🔒 - Get user
- `PUT /api/users/{id}/activate` 👑 - Activate
- `PUT /api/users/{id}/deactivate` 👑 - Deactivate
- `DELETE /api/users/{id}` 👑 - Delete

🔒 = Auth Required  
👑 = Admin Only

---

## ⚡ Example Requests

### Register
```json
POST /api/auth/register
{
  "userName": "newuser",
  "email": "user@example.com",
  "password": "Password123",
  "confirmPassword": "Password123",
  "roleId": "6"
}
```

### Login
```json
POST /api/auth/login
{
  "userName": "admin",
  "password": "Admin@123"
}
```

### Create Role (Admin)
```json
POST /api/roles
Authorization: Bearer {admin_token}
{
  "name": "Custom Role",
  "description": "Description",
  "isAdministrative": false,
  "canEditModel": true
}
```

### Assign Role (Admin)
```json
POST /api/roles/assign
Authorization: Bearer {admin_token}
{
  "userId": "user-id-here",
  "roleId": "2"
}
```

---

## 🔐 Token Info

- **Access Token:** 60 minutes
- **Refresh Token:** 7 days
- **Storage:** Database (can be revoked)

---

## 🧪 Test in Swagger

1. Run: `dotnet run`
2. Open: `https://localhost:5001/swagger`
3. Click "Authorize"
4. Enter: `Bearer {your_token}`
5. Test endpoints!

---

## 📱 JavaScript Example

```javascript
// Login
const response = await fetch('/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    userName: 'admin',
    password: 'Admin@123'
  })
});

const { accessToken, refreshToken } = await response.json();

// Use token
const userResponse = await fetch('/api/auth/me', {
  headers: { 'Authorization': `Bearer ${accessToken}` }
});
```

---

For complete documentation, see `API_DOCUMENTATION.md`
