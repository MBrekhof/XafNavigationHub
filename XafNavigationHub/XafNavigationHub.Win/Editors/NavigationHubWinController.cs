using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Layout;
using Microsoft.Extensions.DependencyInjection;
using RoleChooser.Services;
using XafNavigationHub.Module.Controllers;

namespace XafNavigationHub.Win.Editors;

/// <summary>
/// Initializes the NavigationHubControl when it appears in a DashboardView's ControlDetailItem,
/// and refreshes it in place when the session's active roles change (RoleChooser can't re-navigate
/// to this already-open startup tab on WinForms without opening a duplicate).
/// </summary>
public class NavigationHubWinController : ViewController<DashboardView>
{
    private NavigationHubControl _hubControl;
    private IActiveRoleFilter _roleFilter;

    protected override void OnViewControlsCreated()
    {
        base.OnViewControlsCreated();

        foreach (var item in View.GetItems<ControlViewItem>())
        {
            if (item.Control is NavigationHubControl hubControl)
            {
                _hubControl = hubControl;
                var mainWindow = Application.MainWindow;
                var controller = mainWindow?.GetController<NavigationHubController>();
                if (controller != null)
                {
                    hubControl.Initialize(controller);
                }
            }
        }

        // Re-render the hub when active roles change. On WinForms the RoleChooser signals via
        // SessionRolesApplied instead of re-navigating (which would open a duplicate tab).
        if (_hubControl != null && _roleFilter == null)
        {
            _roleFilter = Application.ServiceProvider.GetService<IActiveRoleFilter>();
            if (_roleFilter != null)
                _roleFilter.SessionRolesApplied += RoleFilter_SessionRolesApplied;
        }
    }

    private void RoleFilter_SessionRolesApplied(object sender, EventArgs e)
        => _hubControl?.RefreshData();

    protected override void Dispose(bool disposing)
    {
        if (disposing && _roleFilter != null)
        {
            _roleFilter.SessionRolesApplied -= RoleFilter_SessionRolesApplied;
            _roleFilter = null;
        }
        base.Dispose(disposing);
    }
}
