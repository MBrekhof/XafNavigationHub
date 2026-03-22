using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;

namespace XafNavigatonHub.Module.BusinessObjects;

[DefaultClassOptions]
[NavigationItem("Sales")]
[ImageName("BO_Product")]
public class Product : BaseObject
{
    public virtual string Name { get; set; }
    public virtual string Sku { get; set; }
    public virtual string Category { get; set; }
    public virtual decimal Price { get; set; }
    public virtual int StockQuantity { get; set; }
    public virtual bool IsDiscontinued { get; set; }
}
