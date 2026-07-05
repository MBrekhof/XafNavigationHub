# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## First Steps — Every Session

1. Read `TODO.md` to understand pending/in-progress work
2. Read `SESSION_HANDOFF.md` to pick up where the last session left off

## Workflow Rules

- **Always plan before executing.** Use the writing-plans skill or brainstorming before implementation.
- **Track work in `TODO.md`.** Update task status as you progress.
- **Update `SESSION_HANDOFF.md`** at the end of every session with current state, next steps, and blockers.
- **Commit and push after every meaningful change.** Keep the remote up to date.
- **Keep `README.md` and `HOW_TO_IMPLEMENT.md` current.** Update them when the project evolves.

## Project Overview

XafNavigationHub is a **Navigation Hub / Launchpad** — a DevExpress XAF application that replaces sidebar navigation with a card-based dashboard. Built on .NET 8 with DevExpress v25.2.3, EF Core, SQL Server (LocalDB), with Blazor Server and WinForms frontends.

## Solution Structure

- **XafNavigationHub.Module** — Platform-agnostic: business objects, controllers, model extensions, database updater. Both frontends reference this.
- **XafNavigationHub.Blazor.Server** — Blazor Server frontend. `NavigationHubComponent.razor` is the hub UI.
- **XafNavigationHub.Win** — WinForms frontend. `NavigationHubControl.cs` is the owner-draw hub UI.

Solution file: `XafNavigationHub.slnx` (XML-based solution format).

## Build & Run

```bash
dotnet build XafNavigationHub.slnx
dotnet run --project XafNavigationHub/XafNavigationHub.Blazor.Server
dotnet run --project XafNavigationHub/XafNavigationHub.Win
```

## Database

- EF Core with `XafNavigationHubEFCoreDbContext` (SQL Server LocalDB)
- Connection string in `appsettings.json`: `(localdb)\mssqllocaldb`, catalog `XafNavigationHub`
- XAF handles schema updates via `ModuleUpdater` (`DatabaseUpdate/Updater.cs`)
- Debug builds seed Admin, HrManager, SalesRep users with empty passwords

## Key Architecture Details

- **Hub hosting**: `DashboardView` with `ControlDetailItem` pointing to platform-specific control
- **Blazor hub**: Razor component using `CascadingParameter` → `BlazorControlViewItem` → `Application.MainWindow` to get controller
- **WinForms hub**: `XtraUserControl` with GDI+ owner-draw, initialized via `NavigationHubWinController`
- **Icon resolution**: `ImageLoader.Instance.GetLargeImageInfo()` → `ImageInfo.ImageBytes` → base64 data URI (Blazor) or `SvgImage.Render()` with `SvgPaletteHelper` (WinForms)
- **WinForms skin colors**: Use `CommonSkins.GetSkin().SvgPalettes[Skin.DefaultSkinPaletteName]` with named colors ("Paint", "Paint High", "Brush", "Paint Shadow"). Do NOT use `DXSkinColors.FillColors` for background/text — it only has semantic highlight colors (Primary, Success, Warning, Danger, Question).
- **Blazor dark theme**: CSS variables `--dxds-color-*` (DX Design System) with `--bs-*` (Bootstrap) fallbacks
- **Permission filtering**: `ShowNavigationItemAction.Items` filtered by `item.Enabled && item.Active`. External URL buttons bypass permission check.
- **Security**: `ApplicationUser` extends `PermissionPolicyUser`. Cookie auth on Blazor.
- **Model config**: Hub layout in `Model.DesignedDiffs.xafml` under `NavigationHub` node. Platform-specific overrides in `Model.xafml` per frontend.
