using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;

namespace XafNavigationHub.Module.BusinessObjects;

[DefaultClassOptions]
[NavigationItem("Project Management")]
[ImageName("BO_Task")]
public class ProjectTask : BaseObject
{
    public virtual string Subject { get; set; }
    public virtual string Description { get; set; }
    public virtual string AssignedTo { get; set; }
    public virtual DateTime DueDate { get; set; }
    public virtual TaskPriority Priority { get; set; }
    public virtual TaskState State { get; set; }
}

public enum TaskPriority
{
    Low,
    Normal,
    High,
    Critical
}

public enum TaskState
{
    NotStarted,
    InProgress,
    Completed,
    Deferred
}
