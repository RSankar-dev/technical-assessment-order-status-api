namespace OrderStatusHub.Models;
public class UnifiedOrder
{
    public string OrderId { get; set; }
    public string SourceSystem { get; set; }
    public string CustomerName { get; set; }
    public string OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; }
}