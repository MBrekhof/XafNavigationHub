# ContextBoard — Done

Completed cards, newest first. Generated from the board.

## 2026-07-05

#### Rename project XafNavigatonHub → XafNavigationHub (typo fix)

Typo fix XafNavigatonHub → XafNavigationHub across solution, projects, namespaces, XafNavigationHubEFCoreDbContext, assemblies, .xafml model, config, docs, GitHub repo (old URL redirects), filesystem folders, and the Claude memory dir key. (XafNavigationHub commit fd565a8.)

#### Upgrade DevExpress 25.2.3 → 25.2.5

Upgraded DevExpress 25.2.3 → 25.2.5. Fixed the Win project NU1605 package downgrade (the converter left Microsoft.Extensions.Configuration at 8.0.0 while bumping Configuration.Json to 9.0.0; aligned Configuration to 9.0.0). Full solution builds green. (XafNavigationHub commit 1545900.)

#### Phase 1: Navigation Hub card dashboard (both platforms)

Navigation Hub card dashboard delivered on both Blazor Server and WinForms. Includes:
- RoleChooser integration PoC (rolechooser branch) — dynamic role switching filters hub cards
- Split demo entities into individual files with [NavigationItem] group attributes
- Fix unpin security error + "x" unpin button on pinned cards (both platforms)
- WinForms dark theme (SVG palette colors); Blazor dark theme (DX Design System CSS variables)
- Demo business objects, roles, users, seed data
- Permission-based button filtering (Enabled/Active check)
- Tab title fix ("Main" instead of "NavigationHub_DashboardView")
- WinForms Hub ViewItem (owner-draw UserControl); non-closable hub tab (DocumentManager TabbedView)
- Drag and drop pinning (HTML5 drag API, Blazor); external URL button support
- Hub card icons via ImageLoader; TabbedMDI enabled both platforms
- Application Model extensions (IModelNavigationHub, IModelHubCategory, IModelHubButton)
- UserHubPreference business object; NavigationHubController (role filtering, navigation, pin CRUD)
- Blazor NavigationHub Razor component; hub registered as DashboardView + startup nav item
- Sample hub configuration in module model; prevent closing hub tab in Blazor TabbedMDI

(Completed before 2026-07-05; migrated to the board this session, so it groups under today's date in DONE.md.)

#### Fix WinForms duplicate-tab crash on RoleChooser role selection

Root cause: `RoleChooserWindowController.ChooseRolesAction_Execute` re-executed the startup navigation item to refresh the hub; on WinForms TabbedMDI that opened a second "Main" DashboardView tab, and the duplicate document crashed DocumentManager layout restore on re-logon ("An item with the same key has already been added ... Text: Main").

Fix is platform-aware: WinForms raises the new `IActiveRoleFilter.SessionRolesApplied` event instead of re-navigating, and `NavigationHubWinController` refreshes the hub in place via `RefreshData()`. Blazor unchanged.

Commits (local only, not pushed): XafRoleChooser 7dbec09 (master), XafNavigationHub 6a59f93 (rolechooser). Verified over 4 logout/login cycles — no crash, one hub tab per login, cards re-filter in place.
