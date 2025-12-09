using OrderStatusHub.Models;

namespace OrderStatusHub.Services;

public interface IOrderIntegrationService
{
    IReadOnlyList<UnifiedOrder> Orders { get; }

    void LoadData();

    UnifiedOrder? GetById(string id);

    IEnumerable<UnifiedOrder> SearchByStatus(string status);
}
