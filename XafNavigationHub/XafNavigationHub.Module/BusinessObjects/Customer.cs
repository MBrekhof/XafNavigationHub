using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;

namespace XafNavigationHub.Module.BusinessObjects;

[DefaultClassOptions]
[NavigationItem("Sales")]
[ImageName("BO_Customer")]
public class Customer : BaseObject
{
    public virtual string CompanyName { get; set; }
    public virtual string ContactName { get; set; }
    public virtual string Email { get; set; }
    public virtual string Phone { get; set; }
    public virtual string City { get; set; }
    public virtual string Country { get; set; }
}
