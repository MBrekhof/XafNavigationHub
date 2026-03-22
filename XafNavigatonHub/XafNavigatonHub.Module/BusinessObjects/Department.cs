using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;

namespace XafNavigatonHub.Module.BusinessObjects;

[DefaultClassOptions]
[NavigationItem("HR")]
[ImageName("BO_Department")]
public class Department : BaseObject
{
    public virtual string Name { get; set; }
    public virtual string Code { get; set; }
    public virtual string Manager { get; set; }
    public virtual string Location { get; set; }
}
