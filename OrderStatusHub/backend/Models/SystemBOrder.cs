namespace OrderStatusHub.Models;
public class SystemBOrder
{
    public string Order_num { get; set; }
    public string Client_name { get; set; }
    public string Date_placed { get; set; }
    public decimal Total { get; set; }
    public int Order_status { get; set; }
}