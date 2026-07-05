using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;

namespace XafNavigationHub.Module.BusinessObjects;

[DefaultClassOptions]
[NavigationItem("HR")]
[ImageName("BO_Employee")]
public class Employee : BaseObject
{
    public virtual string FirstName { get; set; }
    public virtual string LastName { get; set; }
    public virtual string Email { get; set; }
    public virtual string Department { get; set; }
    public virtual string JobTitle { get; set; }
    public virtual DateTime HireDate { get; set; }
    public virtual decimal Salary { get; set; }
    public virtual bool IsActive { get; set; } = true;
}
