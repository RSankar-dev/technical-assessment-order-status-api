namespace OrderStatusHub.Models;
public class SystemAOrder
{
    public string OrderID { get; set; }
    public string Customer { get; set; }
    public string OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; }
}