# RoleChooser Integration Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Integrate the RoleChooser module into NavigationHub so users can switch active roles and see hub cards update dynamically.

**Architecture:** Project reference from NavigationHub.Module to `C:\projects\xafrolechooser\src\RoleChooser\RoleChooser.csproj`. Change `ApplicationUser` base class. Register module + DI in both frontends. Update seed data for multi-role users.

**Tech Stack:** .NET 8, DevExpress XAF 25.2.3, EF Core, SQL Server LocalDB, Blazor Server, WinForms

---

### Task 1: Add Project Reference

**Files:**
- Modify: `XafNavigatonHub/XafNavigatonHub.Module/XafNavigatonHub.Module.csproj`

**Step 1: Add the project reference**

Add to the `<ItemGroup>` in `XafNavigatonHub.Module.csproj`:

```xml
<ProjectReference Include="..\..\..\..\xafrolechooser\src\RoleChooser\RoleChooser.csproj" />
```

This relative path goes from `XafNavigatonHub/XafNavigatonHub.Module/` up to `C:\projects\` then into `xafrolechooser\src\RoleChooser\`.

**Step 2: Verify the project builds**

Run: `dotnet build XafNavigatonHub.slnx`
Expected: Build succeeded, no errors.

**Step 3: Commit**

```bash
git add XafNavigatonHub/XafNavigatonHub.Module/XafNavigatonHub.Module.csproj
git commit -m "feat: add RoleChooser project reference"
```

---

### Task 2: Change ApplicationUser Base Class

**Files:**
- Modify: `XafNavigatonHub/XafNavigatonHub.Module/BusinessObjects/ApplicationUser.cs`

**Step 1: Update the base class**

Change:
```csharp
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
```
to:
```csharp
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using RoleChooser.Security;
```

Change:
```csharp
public class ApplicationUser : PermissionPolicyUser, ISecurityUserWithLoginInfo, ISecurityUserLockout
```
to:
```csharp
public class ApplicationUser : RoleChooserUserBase, ISecurityUserWithLoginInfo, ISecurityUserLockout
```

**Step 2: Verify the project builds**

Run: `dotnet build XafNavigatonHub.slnx`
Expected: Build succeeded. `RoleChooserUserBase` extends `PermissionPolicyUser`, so all existing code compiles unchanged.

**Step 3: Commit**

```bash
git add XafNavigatonHub/XafNavigatonHub.Module/BusinessObjects/ApplicationUser.cs
git commit -m "feat: change ApplicationUser base to RoleChooserUserBase"
```

---

### Task 3: Register RoleChooser in Blazor Startup

**Files:**
- Modify: `XafNavigatonHub/XafNavigatonHub.Blazor.Server/Startup.cs`

**Step 1: Add using and DI registration**

Add at the top of the file:
```csharp
using RoleChooser;
```

In `ConfigureServices()`, add `services.AddRoleChooser();` right before the `services.AddXaf(...)` call (line 33):
```csharp
services.AddRoleChooser();
services.AddXaf(Configuration, builder =>
```

**Step 2: Add module registration**

In the `builder.Modules` chain, add `.Add<RoleChooser.RoleChooserModule>()` before the platform-specific module (line 57-58):
```csharp
.Add<XafNavigatonHub.Module.XafNavigatonHubModule>()
.Add<RoleChooser.RoleChooserModule>()
.Add<XafNavigatonHubBlazorModule>();
```

**Step 3: Verify the project builds**

Run: `dotnet build XafNavigatonHub.slnx`
Expected: Build succeeded.

**Step 4: Commit**

```bash
git add XafNavigatonHub/XafNavigatonHub.Blazor.Server/Startup.cs
git commit -m "feat: register RoleChooser module in Blazor startup"
```

---

### Task 4: Register RoleChooser in WinForms Startup

**Files:**
- Modify: `XafNavigatonHub/XafNavigatonHub.Win/Startup.cs`

**Step 1: Add using and DI registration**

Add at the top of the file:
```csharp
using RoleChooser;
```

In `BuildApplication()`, add `builder.Services.AddRoleChooser();` right before `builder.UseApplication<...>()` (line 28):
```csharp
builder.Services.AddRoleChooser();
builder.UseApplication<XafNavigatonHubWindowsFormsApplication>();
```

**Step 2: Add module registration**

In the `builder.Modules` chain, add `.Add<RoleChooser.RoleChooserModule>()` before the platform-specific module (line 53-54):
```csharp
.Add<XafNavigatonHub.Module.XafNavigatonHubModule>()
.Add<RoleChooser.RoleChooserModule>()
.Add<XafNavigatonHubWinModule>();
```

**Step 3: Verify the project builds**

Run: `dotnet build XafNavigatonHub.slnx`
Expected: Build succeeded.

**Step 4: Commit**

```bash
git add XafNavigatonHub/XafNavigatonHub.Win/Startup.cs
git commit -m "feat: register RoleChooser module in WinForms startup"
```

---

### Task 5: Update Seed Data for Multi-Role Users

**Files:**
- Modify: `XafNavigatonHub/XafNavigatonHub.Module/DatabaseUpdate/Updater.cs`

**Step 1: Give Admin all roles**

In `UpdateDatabaseAfterUpdateSchema()`, after the Admin user is created (around line 57-66), add both HR and Sales roles to Admin:

```csharp
if (userManager.FindUserByName<ApplicationUser>(ObjectSpace, "Admin") == null)
{
    string EmptyPassword = "";
    _ = userManager.CreateUser<ApplicationUser>(ObjectSpace, "Admin", EmptyPassword, (user) =>
    {
        user.Roles.Add(adminRole);
        user.Roles.Add(defaultRole);
        user.Roles.Add(hrRole);
        user.Roles.Add(salesRole);
    });
}
```

Note: `hrRole` and `salesRole` must be created before Admin user. Move the `CreateHrRole()` and `CreateSalesRole()` calls before the user creation block.

**Step 2: Give HrManager both HR and Sales roles**

Update the HrManager creation (around line 75-82):
```csharp
if (userManager.FindUserByName<ApplicationUser>(ObjectSpace, "HrManager") == null)
{
    _ = userManager.CreateUser<ApplicationUser>(ObjectSpace, "HrManager", "", (user) =>
    {
        user.Roles.Add(defaultRole);
        user.Roles.Add(hrRole);
        user.Roles.Add(salesRole);
    });
}
```

**Step 3: Delete the existing LocalDB database** (so seed data re-runs)

Run: `sqlcmd -S "(localdb)\mssqllocaldb" -Q "DROP DATABASE IF EXISTS [XafNavigatonHub]"`

If `sqlcmd` is not available, the app will auto-create a fresh DB on first run since the old one's users won't match.

**Step 4: Verify the project builds**

Run: `dotnet build XafNavigatonHub.slnx`
Expected: Build succeeded.

**Step 5: Commit**

```bash
git add XafNavigatonHub/XafNavigatonHub.Module/DatabaseUpdate/Updater.cs
git commit -m "feat: give demo users multiple roles for RoleChooser testing"
```

---

### Task 6: Smoke Test — Blazor

**Step 1: Run the Blazor app**

Run: `dotnet run --project XafNavigatonHub/XafNavigatonHub.Blazor.Server`
Expected: App starts, database is created/updated.

**Step 2: Log in as HrManager (empty password)**

Expected: Navigation Hub shows cards for HR items AND Sales items (Employee, Department, Customer, SalesOrder, etc.)

**Step 3: Click "Active Roles" in toolbar**

Expected: Popup shows checkboxes for HR and Sales roles (Default is always-active and hidden).

**Step 4: Uncheck "Sales" role, click OK**

Expected: App reloads. Hub now shows only HR-related cards (Employee, Department, ProjectTask, AuditLogEntry). Sales cards (Customer, SalesOrder, Product) are gone.

**Step 5: Re-enable Sales role**

Expected: All cards return.

---

### Task 7: Smoke Test — WinForms

**Step 1: Run the WinForms app**

Run: `dotnet run --project XafNavigatonHub/XafNavigatonHub.Win`

**Step 2: Repeat the same test as Task 6** with HrManager user.

Expected: Same behavior — "Active Roles" button in ribbon, role toggle filters hub cards.
