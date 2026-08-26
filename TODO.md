# ContextBoard — TODO

Generated from the board (source of truth) — do not hand-edit; run export_markdown.

## Todo

#### NAV-001: Phase 2 — runtime admin UI for hub config

Replace Model-Editor-only hub configuration with CRUD business objects (categories / buttons / pins) so the hub layout can be edited at runtime instead of in `Model.DesignedDiffs.xafml`.

#### BUILD-001: Decouple RoleChooser before merge to main

Prerequisite for merging `rolechooser` → `main`. Remaining blocker:

1. `XafNavigationHub.Module.csproj` references `..\..\..\XafRoleChooser\src\RoleChooser\RoleChooser.csproj` — an out-of-repo relative path, so fresh clones / CI / other machines without the sibling repo can't build the solution.

~~2. The app depends on RoleChooser's `IActiveRoleFilter.SessionRolesApplied`, which existed only in a local, unpushed commit (7dbec09).~~ Resolved: XafRoleChooser master (incl. 7dbec09 and the 26.1.4/net10 upgrade 9e61a03) is pushed to origin.

Since 2026-08-26 both repos are on net10.0 / DevExpress 26.1.4 / EF Core 10.0.11 (UPG-001), so the project reference builds again — but it still couples the Hub to whatever XAF major/minor the sibling checkout happens to be on. Decouple by one of: package RoleChooser as a NuGet (local/private feed) referenced by version (cleanest, unblocks CI); git submodule the XafRoleChooser repo into this one; or vendor the source. Do this once RC-001 is confirmed and a merge is actually wanted. Fix HUB-001 first if the merged branch is expected to work on Blazor with RoleChooser.

#### HUB-001: Blazor hub cards do not re-filter after RoleChooser role selection

Found 2026-08-26 while smoke-testing the 26.1.4 / net10 upgrade (UPG-001), Blazor, Admin login, pick only "HR" in the Active Role Selection popup → OK.

Observed: the sidebar navigation re-filters correctly (Sales / Reports groups disappear), but the hub cards keep showing Sales & CRM (Customers, Orders, Products) and Administration (Users, Roles). The toolbar Refresh action does not re-filter them either. WinForms is fine (6a59f93: NavigationHubWinController subscribes to IActiveRoleFilter.SessionRolesApplied → RefreshData()).

Root cause: `NavigationHubComponent.razor` reads `controller.GetHubData()` only in `OnInitialized` (plus its own pin/unpin). RoleChooser's Blazor path re-executes the startup navigation item (`ShowNavigationItemAction.DoExecute(startupItem)`), which does not recreate the already-open hub component, so nothing re-reads. RoleChooser raises `SessionRolesApplied` only on WinForms (`IsWinFormsApplication` branch in RoleChooserWindowController.ChooseRolesAction_Execute).

Proposed fix (two repos):
1. XafRoleChooser: call `_roleFilter.NotifySessionRolesApplied()` on every platform (move it out of the WinForms-only branch; keep the Blazor re-execute for consumers without an in-place hub).
2. Hub Blazor: `NavigationHubComponent` resolves `IActiveRoleFilter` from `ViewItem.Application.ServiceProvider`, subscribes to `SessionRolesApplied` → `RefreshData(); InvokeAsync(StateHasChanged)`, unsubscribes in `Dispose`. Same pattern as the Win controller.

This is RoleChooser's RC-008 b) ("verify hub cards refresh after role switch") — now confirmed as a bug, not a verification item. Pre-existing, not an upgrade regression (nothing in the upgrade touched this path).

## Review

#### RC-001: WinForms RoleChooser multi-select parity

The login-time "Active Roles" chooser works on WinForms now (no longer crashes), but you can only select ONE role — the generic `Application.CreateListView` grid defaults to single-row select, unlike Blazor's checkbox list. Worse, `ChooseRolesAction_Execute` reads the selected ROWS (`PopupWindowViewSelectedObjects`) rather than the `ActiveRoleSelection.IsActive` column, so even the visible checkbox column wouldn't drive the result.

Fix in the RoleChooser library: enable grid multi-select / checkbox-row mode on WinForms, or switch the Execute handler to read `IsActive` instead of row selection.

Repo: C:\projects\XafRoleChooser — src/RoleChooser/Controllers/RoleChooserWindowController.cs (shared lib, master). Tracked there as RC-007 (card 1190).

---
**2026-08-26, Hub on 26.1.4 (UPG-001):** RoleChooser's RC-007 fix (`1c90494`) is in the referenced lib, but on DevExpress 26.1 WinForms it throws `AmbiguousMatchException` (`GridView.OptionsSelection` is re-declared in 26.1, `Type.GetProperty(name)` is ambiguous) inside `ListView.ControlsCreated`. Effect in the Hub: after Admin login the **hub tab shows the exception text instead of the cards**. Details + fix on RoleChooser card 1190. Do not confirm this card until RoleChooser has re-verified on 26.1 WinForms and the Hub's hub tab renders for Admin again.

**Outcome:** Not delivered — closed as a duplicate/misfile, no code change. This is a RoleChooser library fix (src/RoleChooser/Controllers/RoleChooserWindowController.cs), not a NavigationHub one, and it was filed here as "RC-001" which collides with an already-completed card on the XafRoleChooser board. Re-filed on XafRoleChooser (project 10) as card #1190, "RC-007: WinForms RoleChooser multi-select parity", and cited from that repo's TODO.md. Confirm-Done here to retire the duplicate; the work itself is still open on #1190.

#### UPG-001: Upgrade to .NET 10, DevExpress 26.1.4, EF Core 10

Bring the Hub in line with XafRoleChooser (9e61a03) and WLNCentral: net8.0 -> net10.0 (Win: net10.0-windows), DevExpress.* 25.2.5 -> 26.1.4, Microsoft.EntityFrameworkCore.* 8.0.18 -> 10.0.11, Microsoft.CodeAnalysis.* 4.10.0 -> 5.0.0, Microsoft.Data.SqlClient 6.1.6, Microsoft.Extensions.Configuration 10.x.

Known 26.1 changes from the RoleChooser upgrade: WinApplication.UseOldTemplates removed; password hashing moved to SHA512/600K under CompatibilityMode.Latest (enable PasswordCryptographer.UseSHA1_20K + UseSHA512_600K so pre-upgrade LocalDB users still log in).

Motivation: the `rolechooser` branch project-references RoleChooser, which is already net10.0 / 26.1.4 — the Hub cannot build until it follows. .NET 8 EOL 2026-11-10.

**Outcome:** Commits 5411b15 (csproj TFM/package bumps, UseOldTemplates removal, PasswordCryptographer toggles), 8fcead1 (XAF0035: SecuritySystem.CurrentUserId → Application.Security.UserId), 8fa2ac5 (docs) on branch `rolechooser`. Build: 0 errors, 0 warnings in Hub code. Verified at runtime against the existing LocalDB: Blazor (Playwright — Admin login with pre-upgrade hash, hub renders, role chooser popup, 0 console errors) and WinForms (HrManager login, hub renders with icons; Admin login logs on but the hub tab shows RoleChooser's RC-007 AmbiguousMatchException — RoleChooser card 1190 / Hub RC-001 annotated, not a Hub change). Also found HUB-001 (Blazor hub cards do not re-filter after role selection; pre-existing). DB needed no schema update.

