# Session Handoff

## Current State

**Branch: `rolechooser`** — Phase 1 + RoleChooser integration, now on **.NET 10 / DevExpress 26.1.4 /
EF Core 10.0.11** (UPG-001, 2026-08-26). Both platforms build clean (0 errors, 0 warnings in our
code) and both were smoke-tested at runtime against the existing `XafNavigationHub` LocalDB.

Open work lives on the ContextBoard (project 11); `TODO.md` / `docs/DONE.md` are board exports.

## Done this session (2026-08-26)

**UPG-001 — upgrade to .NET 10 / DX 26.1.4 / EF Core 10** (`5411b15`, `8fcead1`, docs commit after).

- Why now: the sibling `XafRoleChooser` repo moved to net10.0 / 26.1.4 this morning (`9e61a03`),
  and this branch project-references it — the Hub could not build until it followed. .NET 8 is
  EOL 2026-11-10 anyway.
- Recipe (identical to RoleChooser's): TFM `net8.0` → `net10.0` (Win `net10.0-windows`);
  `DevExpress.*` 25.2.5 → 26.1.4; `Microsoft.EntityFrameworkCore.*` 8.0.18 → 10.0.11;
  `Microsoft.CodeAnalysis.*` 4.10.0 → 5.0.0; `Microsoft.Data.SqlClient` 6.1.2 → 6.1.6;
  Win `Microsoft.Extensions.Configuration(.Json)` 9.0.0 → 10.0.11.
- 26.1 code changes: `WinApplication.UseOldTemplates` removed (line deleted);
  `PasswordCryptographer.UseSHA1_20K` + `UseSHA512_600K` enabled in both `Program.cs` — 26.1 hashes
  with SHA512/600K under `CompatibilityMode.Latest` and would otherwise refuse the pre-upgrade
  users. Verified: Admin (empty password, SHA1-era row) logs in on both platforms.
- `XAF0035` (new 26.1 analyzer warning): `SecuritySystem.CurrentUserId` in the shared
  `NavigationHubController` → `Application.Security.UserId` (per-circuit `ISecurityStrategyBase`,
  docs 405775).
- Verified: Blazor — login, hub renders all categories/cards, RoleChooser popup, 0 console errors
  (Playwright). WinForms — starts, logs on (SendKeys-driven), **hub renders for `HrManager`**
  (screenshot: Human Resources + Sales & CRM cards, icons OK). **For `Admin` the hub tab shows an
  exception instead of cards — see RC-001/RC-007 below; that is RoleChooser's code, not the Hub's.**
  `eXpressAppFramework.log` reports 0 exceptions in both cases (XAF swallows that one into the
  document), so a screenshot is the only real check on WinForms.
- The DB schema needed no update (XAF started against the 25.2-era catalog without a
  `DatabaseVersionMismatch`).

**Found while testing — RoleChooser RC-007 fails on 26.1 WinForms** (RoleChooser card 1190, in
Review; Hub card RC-001 / 378 annotated): `EnableCheckBoxRowSelect` does
`gridView.GetType().GetProperty("OptionsSelection")`; in 26.1 `GridView` re-declares
`OptionsSelection` (`new`, `GridOptionsSelection`) over the `ColumnView` one, so reflection throws
`AmbiguousMatchException` inside `ListView.ControlsCreated`. Effect: Admin's hub tab on WinForms
renders `Exception occurs while assigning the 'ListView, ID:ActiveRoleSelection_ListView' ...
Ambiguous match found ...`. Fix is a one-liner in the lib (take the most-derived declaration);
sketch on card 1190. Not a Hub change.

**HUB-001 — found and fixed (card 1415, in Review):** on **Blazor** the hub cards did not
re-filter after choosing roles in the login-time chooser (sidebar re-filtered, hub kept Sales & CRM
+ Administration; toolbar Refresh didn't help). Root cause: `NavigationHubComponent.razor` read
`GetHubData()` only in `OnInitialized`; RoleChooser's Blazor path re-executes the startup nav item,
which does not recreate an already-open component, and it raised `SessionRolesApplied` on WinForms
only. Fix: XafRoleChooser `c7b1932` raises the event on every platform (before the kept Blazor
re-execute); Hub `e7db2a1` makes the component subscribe (`IActiveRoleFilter` from
`ViewItem.Application.ServiceProvider`, `InvokeAsync(RefreshData + StateHasChanged)`, unsubscribe in
`Dispose`) — same pattern as `NavigationHubWinController`. Verified with Playwright: pick HR only →
Sales & CRM and Roles cards vanish in place, 0 console errors. Pre-existing (= RoleChooser RC-008 b).

**Board housekeeping:** BUILD-001 body updated — blocker 2 (unpushed RoleChooser commit) is gone,
RoleChooser master is on origin; blocker 1 (out-of-repo project reference) remains.

## Next Steps

- **RoleChooser RC-007 / Hub RC-001 — fixed, one check left.** RoleChooser (master, pushed)
  now resolves `OptionsSelection` most-derived-first; verified in this Hub's Win app: Admin logon →
  chooser with checkbox column → OK → hub renders, 0 exceptions (screenshot). **Not yet seen:
  ticking two roles with the mouse and the hub re-filtering** — synthetic input didn't reach the
  modal chooser from scripts (details in RoleChooser's handoff); quickest is by hand: run
  `XafNavigationHub.Win`, log in as Admin, tick HR + Sales, OK, expect HR/Sales cards only. Then
  confirm RC-001 (378) here and RC-007 (1190) on the RoleChooser board.
- **HUB-001** — done; sits in Review for your confirm.
- **BUILD-001** — decouple RoleChooser (NuGet / submodule / vendor) before merging to `main`.
- **NAV-001** — Phase 2 runtime admin UI for hub config.

## Notes

- Blazor smoke recipe: from `XafNavigationHub/XafNavigationHub.Blazor.Server`, run
  `ASPNETCORE_ENVIRONMENT=Development ASPNETCORE_URLS=http://localhost:5000 bin/Debug/net10.0/XafNavigationHub.Blazor.Server.exe`;
  Admin / empty password; hub at `/NavigationHub_DashboardView`. WinForms exe:
  `XafNavigationHub/XafNavigationHub.Win/bin/Debug/net10.0-windows/XafNavigationHub.Win.exe`.
- EF Core still warns at startup about unspecified decimal store types on the demo entities
  (`Employee.Salary`, `Product.Price`, `SalesOrder.TotalAmount`) — pre-existing, cosmetic for a
  demo, add `[Precision(18,2)]` if it ever matters.
- WinForms user layout is persisted **in the DB** (`ModelDifferenceDbStore(..., "Win")` in
  `WinModule.cs`), not a file. If a corrupted two-"Main"-tab layout ever recurs, clear the `Win`
  row in `ModelDifferences` / `ModelDifferenceAspects` for the affected user.
- Decision stands: **not merged to main** — the cross-repo project reference to
  `..\..\..\XafRoleChooser` breaks standalone builds (BUILD-001).
