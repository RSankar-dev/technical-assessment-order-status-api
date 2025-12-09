using CsvHelper.Configuration;
using OrderStatusHub.Models;

namespace OrderStatusHub.Api.Utilities
{
    // CsvHelper mapping class
    public sealed class SystemBMap : ClassMap<SystemBOrder>
    {
        public SystemBMap()
        {
            Map(m => m.Order_num).Name("order_num");
            Map(m => m.Client_name).Name("client_name");
            Map(m => m.Date_placed).Name("date_placed");
            Map(m => m.Total).Name("total");
            Map(m => m.Order_status).Name("order_status");
        }
    }
}
