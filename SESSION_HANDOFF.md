# Session Handoff

## Current State

**Branch: `rolechooser`** — Phase 1 complete + RoleChooser integration PoC.

### What's Implemented

- **Navigation Hub** — card-based dashboard replacing sidebar navigation, registered as DashboardView startup item
- **Model extensions** — `IModelNavigationHub`, `IModelHubCategory`, `IModelHubButton` with `ExternalUrl` support
- **NavigationHubController** (Module) — role-filtered hub data via `ShowNavigationItemAction`, programmatic navigation, per-user pin CRUD via `UserHubPreference`
- **Blazor frontend** — `NavigationHubComponent.razor` with drag & drop pinning, external URLs via JSInterop, dark theme via `--dxds-*` CSS variables with `--bs-*` fallbacks
- **WinForms frontend** — `NavigationHubControl` (owner-draw `XtraUserControl` with GDI+ painting), SVG icon rendering via `SvgPaletteHelper`, dark theme via `CommonSkins.GetSkin().SvgPalettes` palette colors ("Paint", "Paint High", "Brush", "Paint Shadow")
- **Non-closable hub tab** — `HubTabController` (Blazor) and `HubTabWinController` (Win)
- **Demo data** — 7 business objects in individual files with `[NavigationItem]` group attributes (HR, Sales, Project Management), seed data, multi-role users
- **Permission filtering** — buttons hidden based on `ChoiceActionItem.Enabled && Active` state, external URL buttons bypass permission check
- **RoleChooser integration** — XafRoleChooser module integrated via project reference. Users can switch active roles at runtime; hub cards update dynamically

### RoleChooser Integration Details

- `ApplicationUser` base class changed from `PermissionPolicyUser` to `RoleChooserUserBase` (no schema impact)
- `RoleChooserModule` + `AddRoleChooser()` registered in both Blazor and WinForms startup
- Project reference: `../../../XafRoleChooser/src/RoleChooser/RoleChooser.csproj` (cross-repo, requires XafRoleChooser at `C:\projects\XafRoleChooser`)
- Demo users: Admin has all 4 roles, HrManager has Default+HR+Sales, SalesRep has Default+Sales
- Blazor confirmed working. WinForms needs manual verification.
- Decision: **not merged to main** — cross-repo project reference breaks standalone builds. Branch kept as reference for wlncentral integration.

### Latest Changes

- Integrated XafRoleChooser module (project reference, base class, DI, module registration)
- Split `DemoEntities.cs` into 7 individual files with `[NavigationItem]` attributes
- Updated nav permission paths in Updater.cs to match new groups (HR, Sales, Project Management)
- Updated HOW_TO_IMPLEMENT.md with RoleChooser section and NavigationItem path notes

## Next Steps

- **wlncentral integration**: Bring both NavigationHub and RoleChooser modules into `C:\projects\wlncentral`
- **Phase 2**: Runtime admin UI for hub configuration (business objects instead of Model Editor)
- Optional: Blazor drag & drop for WinForms (WinForms currently supports right-click pin/unpin only)
- Optional: CSS isolation for Blazor component (`.razor.css`)
