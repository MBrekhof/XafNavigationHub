# RoleChooser Integration Design

**Date:** 2026-03-22
**Branch:** `rolechooser`
**Goal:** Prove that XafRoleChooser works with XafNavigatonHub — switching roles dynamically filters hub cards.

## Approach

Project reference from NavigationHub.Module to the RoleChooser module at `C:\projects\xafrolechooser\src\RoleChooser\RoleChooser.csproj`. No code changes to either module's internals.

## Changes Required

### 1. Project Reference

`XafNavigatonHub.Module.csproj` gets a `<ProjectReference>` to the RoleChooser project (relative path).

### 2. ApplicationUser Base Class

`XafNavigatonHub.Module/BusinessObjects/ApplicationUser.cs`:
- Change `PermissionPolicyUser` to `RoleChooserUserBase`
- Add `using RoleChooser.Security;`

**Schema impact: NONE.** `RoleChooserUserBase` adds zero properties — it only overrides the `Roles` getter with filtering logic. No migration needed.

### 3. Blazor Startup

`XafNavigatonHub.Blazor.Server/Startup.cs`:
- `services.AddRoleChooser();` in `ConfigureServices()`
- `.Add<RoleChooserModule>()` in `builder.Modules` chain

### 4. WinForms Startup

`XafNavigatonHub.Win/Startup.cs`:
- `builder.Services.AddRoleChooser();` in `BuildApplication()`
- `.Add<RoleChooserModule>()` in `builder.Modules` chain

### 5. Seed Data

Update `Updater.cs` to give demo users multiple roles so role switching is meaningful (e.g., Admin gets all roles, HrManager gets both HR + Sales).

## What We Don't Change

- No NavigationHub controller/component changes
- No RoleChooser code changes
- No model or XAFML changes
- No new UI elements

## Expected Behavior

1. User logs in -> hub shows cards based on all assigned roles
2. User clicks "Active Roles" in toolbar -> popup shows role checkboxes
3. User deactivates a role -> RoleChooser reloads permissions, rebuilds navigation, navigates to startup
4. Hub reloads -> cards for deactivated role's items are gone
5. User reactivates role -> cards reappear

## Prerequisites

- Both projects already use `PermissionsReloadMode.NoCache`
- Both target DevExpress 25.2.3, .NET 8, EF Core
- No version conflicts

## Future: wlncentral Integration

When bringing both modules into `C:\projects\wlncentral`:
- No SQL migration needed for RoleChooserUserBase (no schema changes)
- Same 4-step integration pattern applies
- Decision needed on packaging (project reference, NuGet, or inline)
