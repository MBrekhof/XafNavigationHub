using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;

namespace XafNavigationHub.Module.BusinessObjects;

[DefaultClassOptions]
[NavigationItem("Sales")]
[ImageName("BO_Order")]
public class SalesOrder : BaseObject
{
    public virtual string OrderNumber { get; set; }
    public virtual DateTime OrderDate { get; set; }
    public virtual string CustomerName { get; set; }
    public virtual decimal TotalAmount { get; set; }
    public virtual OrderStatus Status { get; set; }
}

public enum OrderStatus
{
    Draft,
    Confirmed,
    Shipped,
    Delivered,
    Cancelled
}
