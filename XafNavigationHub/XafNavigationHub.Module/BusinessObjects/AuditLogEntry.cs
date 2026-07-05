using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;

namespace XafNavigationHub.Module.BusinessObjects;

[DefaultClassOptions]
[NavigationItem("Project Management")]
[ImageName("BO_Audit_ChangeHistory")]
public class AuditLogEntry : BaseObject
{
    public virtual DateTime Timestamp { get; set; }
    public virtual string UserName { get; set; }
    public virtual string Action { get; set; }
    public virtual string EntityType { get; set; }
    public virtual string Details { get; set; }
}
