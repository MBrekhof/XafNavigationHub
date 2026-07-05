# ContextBoard — TODO

Generated from the board (source of truth) — do not hand-edit; run export_markdown.

## Todo

#### RC-001: WinForms RoleChooser multi-select parity

The login-time "Active Roles" chooser works on WinForms now (no longer crashes), but you can only select ONE role — the generic `Application.CreateListView` grid defaults to single-row select, unlike Blazor's checkbox list. Worse, `ChooseRolesAction_Execute` reads the selected ROWS (`PopupWindowViewSelectedObjects`) rather than the `ActiveRoleSelection.IsActive` column, so even the visible checkbox column wouldn't drive the result.

Fix in the RoleChooser library: enable grid multi-select / checkbox-row mode on WinForms, or switch the Execute handler to read `IsActive` instead of row selection.

Repo: C:\projects\XafRoleChooser — src/RoleChooser/Controllers/RoleChooserWindowController.cs (shared lib, master).

#### NAV-001: Phase 2 — runtime admin UI for hub config

Replace Model-Editor-only hub configuration with CRUD business objects (categories / buttons / pins) so the hub layout can be edited at runtime instead of in `Model.DesignedDiffs.xafml`.

#### BUILD-001: Decouple RoleChooser before merge to main

Prerequisite for merging `rolechooser` → `main`. Two blockers today:

1. `XafNavigationHub.Module.csproj` references `..\..\..\XafRoleChooser\src\RoleChooser\RoleChooser.csproj` — an out-of-repo relative path, so fresh clones / CI / other machines without the sibling repo can't build the solution.
2. The app depends on RoleChooser's `IActiveRoleFilter.SessionRolesApplied`, which currently exists only in a local, unpushed commit (7dbec09) of the separate XafRoleChooser repo.

Decouple by one of: package RoleChooser as a NuGet (local/private feed) referenced by version (cleanest, unblocks CI); git submodule the XafRoleChooser repo into this one; or vendor the source. Do this once RC-001 (WinForms multi-select) is done and a merge is actually wanted.
