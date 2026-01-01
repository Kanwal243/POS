# CRITICAL FIX - Session Loading Issue

## The Problem

The `LoginManager` was using an `_initialized` flag that prevented session from being reloaded on subsequent requests. Since `LoginManager` is **Scoped** (new instance per request), each request needs to load the session fresh.

## The Fix

Changed the initialization check from:
```csharp
// OLD - Prevented re-loading if already initialized
if (_initialized) return;
```

To:
```csharp
// NEW - Allow re-initialization if user is not loaded
if (_initialized && _currentUser != null) return;
```

This ensures that even if initialized, if `_currentUser` is null, it will try to load from session again.

## How to Test

### Step 1: Rebuild
```powershell
dotnet build
```

### Step 2: Run Application
```powershell
dotnet run
```

### Step 3: Login and Watch Console

After logging in, you should see:
```
LoginManager.InitializeAsync: HttpContext = True, IsInitialized = False, CurrentUser = 
LoginManager: Session available, userJson length = 245
LoginManager: ✅ User loaded from session: admin, Role: Administrator
LoginManager: Initialization complete. IsLoggedIn = True, User = admin
MainLayout: LoginManager.IsLoggedIn = True, User = admin
```

### Step 4: Navigate to Debug Page

1. After login, navigate to: `http://localhost:5000/debug-session`
2. You should see:
   - ✅ **IsLoggedIn**: `True`
   - ✅ **CurrentUser**: `admin` (or your username)
   - ✅ **User Role**: `Administrator` (or your role)
   - ✅ **AuthToken**: `eyJhbGciOiJIUzI1...`
   - ✅ **Session Available**: `True`  
   - ✅ **User in Session**: `Length: 245 chars`

### Step 5: Navigate to Dashboard

Click "Go to Dashboard" and verify:
- ✅ Sidebar appears
- ✅ Header appears  
- ✅ User profile dropdown works
- ✅ All navigation works

### Step 6: Click Sub-Menu Items

1. Click "Default" (should expand)
2. Click "My Details" or "Users"
3. **Watch console** - should see:
   ```
   LoginManager.InitializeAsync: HttpContext = True, IsInitialized = True, CurrentUser = admin
   LoginManager: Already initialized with user, CurrentUser = admin
   ```
4. **Verify** - Sidebar and header should remain visible

## Expected Console Output

### On Every Page Load:
```
LoginManager.InitializeAsync: HttpContext = True, IsInitialized = False, CurrentUser = 
LoginManager: Session available, userJson length = 245
LoginManager: ✅ User loaded from session: [username], Role: [role]
LoginManager: Initialization complete. IsLoggedIn = True, User = [username]
```

### OR (if already loaded in same request):
```
LoginManager: Already initialized with user, CurrentUser = [username]
```

## If Still Not Working

### Scenario 1: Console shows "userJson length = 0"

**Issue**: Session is not being saved during login

**Fix**:
1. Check if `/api/Auth/set-session` is returning 200 OK
2. Verify JavaScript `saveSession` function is executing
3. Check browser DevTools → Network tab for the POST request

### Scenario 2: Console shows "Session not available"

**Issue**: Session middleware not configured properly

**Fix**:
1. Verify `app.UseSession()` is called in `Startup.cs` (line 187)
2. Check if session service is registered (line 121-128)
3. Restart application

### Scenario 3: Sidebar appears then disappears

**Issue**: Second render is clearing the layout

**Fix**:
1. Check if `StateHasChanged()` is being called too many times
2. Verify `LoginManager.IsLoggedIn` isn't changing between renders
3. Add logging in `MainLayout.razor` to see when `@if` condition changes

## Quick Test Commands

### Check if session is being saved:
```csharp
// Add to AuthController.SetSession (line 185)
Console.WriteLine($"SetSession: Saving user {request.User.UserName} to session");
```

### Check if session is being loaded:
```csharp
// Already added in LoginManager.InitializeAsync
// Look for: "✅ User loaded from session: [username]"
```

### Check middleware initialization:
```csharp
// Add to Startup.cs middleware (line 194)
Console.WriteLine($"Middleware: Initializing LoginManager, User before: {loginManager.CurrentUser?.UserName}");
// After line 194
Console.WriteLine($"Middleware: User after init: {loginManager.CurrentUser?.UserName}");
```

## Debug Checklist

- [ ] Console shows "✅ User loaded from session"
- [ ] `IsLoggedIn = True` in console
- [ ] Debug page shows user is loaded
- [ ] Sidebar visible after login
- [ ] Header visible after login
- [ ] Navigation doesn't lose layout
- [ ] Page refresh maintains session
- [ ] Sub-menu navigation works

## Last Resort: Clear Everything

If nothing works:

1. **Stop application**
2. **Delete session database** (if using persistent sessions)
3. **Clear browser cookies**:
   - Press F12
   - Application tab → Cookies → Delete all
4. **Clear browser cache**: Ctrl+Shift+Delete
5. **Rebuild**: `dotnet clean && dotnet build`
6. **Restart**: `dotnet run`
7. **Try in Incognito window**

---

**Date**: 2026-01-01
**Status**: CRITICAL FIX APPLIED
