using CsvHelper;
using CsvHelper.Configuration;
using OrderStatusHub.Api.Utilities;
using OrderStatusHub.Models;
using System.Globalization;
using System.Text.Json;

namespace OrderStatusHub.Services;

public class OrderIntegrationService : IOrderIntegrationService
{
    private readonly ILogger<OrderIntegrationService> _logger;
    private List<UnifiedOrder> _orders = new();
    private readonly IWebHostEnvironment _env;

    public IReadOnlyList<UnifiedOrder> Orders => _orders.AsReadOnly();

    public OrderIntegrationService(ILogger<OrderIntegrationService> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    // Call at startup
    public void LoadData()
    {
        try
        {
            var basePath = Path.Combine(_env.ContentRootPath, "data");

            // If not found (local dev scenario), go one level up
            if (!Directory.Exists(basePath))
            {
                var parentPath = Path.Combine(_env.ContentRootPath, "..", "data");
                if (Directory.Exists(parentPath))
                    basePath = parentPath;
            }

            _logger.LogInformation("Resolved data path: {path}", basePath);

            var aPath = Path.Combine(basePath, "system_a_orders.json");
            var bPath = Path.Combine(basePath, "system_b_orders.csv");

            var list = new List<UnifiedOrder>();

            // -----------------------
            // Load System A JSON
            // -----------------------
            if (File.Exists(aPath))
            {
                _logger.LogInformation("Loading System A data from {file}", aPath);

                var json = File.ReadAllText(aPath);
                var aOrders = JsonSerializer.Deserialize<List<SystemAOrder>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new List<SystemAOrder>();

                foreach (var a in aOrders)
                {
                    list.Add(new UnifiedOrder
                    {
                        OrderId = a.OrderID,
                        SourceSystem = "SystemA",
                        CustomerName = a.Customer,
                        OrderDate = NormalizeDateFromSystemA(a.OrderDate),
                        TotalAmount = a.TotalAmount,
                        Status = MapStatusFromSystemA(a.Status)
                    });
                }
            }
            else
            {
                _logger.LogWarning("System A file not found: {file}", aPath);
            }

            // -----------------------
            // Load System B CSV
            // -----------------------
            if (File.Exists(bPath))
            {
                _logger.LogInformation("Loading System B data from {file}", bPath);

                using var reader = new StreamReader(bPath);
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    MissingFieldFound = null
                };

                using var csv = new CsvReader(reader, config);
                csv.Context.RegisterClassMap<SystemBMap>();
                var bOrders = csv.GetRecords<SystemBOrder>().ToList();

                foreach (var b in bOrders)
                {
                    list.Add(new UnifiedOrder
                    {
                        OrderId = b.Order_num,
                        SourceSystem = "SystemB",
                        CustomerName = b.Client_name,
                        OrderDate = NormalizeDateFromSystemB(b.Date_placed),
                        TotalAmount = b.Total,
                        Status = MapStatusFromSystemB(b.Order_status)
                    });
                }
            }
            else
            {
                _logger.LogWarning("System B file not found: {file}", bPath);
            }

            // -----------------------
            // Finalize
            // -----------------------
            _orders = list.OrderBy(o => o.OrderDate).ToList();
            _logger.LogInformation("Loaded {count} unified orders.", _orders.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading order data");
            throw; // Re-throw so you see failed startup in Azure logs
        }
    }

    private string NormalizeDateFromSystemA(string input)
    {
        // System A uses ISO (yyyy-MM-dd) per spec; parse and ensure format
        if (DateTime.TryParse(input, out var dt))
            return dt.ToString("yyyy-MM-dd");
        return input; // fallback
    }

    private string NormalizeDateFromSystemB(string input)
    {
        // System B uses US format: MM/DD/YYYY -> parse with en-US
        if (DateTime.TryParse(input, new CultureInfo("en-US"), DateTimeStyles.None, out var dt))
            return dt.ToString("yyyy-MM-dd");
        return input;
    }

    private string MapStatusFromSystemA(string code)
    {
        return code?.ToUpperInvariant() switch
        {
            "PEND" => "Pending",
            "PROC" => "Processing",
            "SHIP" => "Shipped",
            "COMP" => "Completed",
            "CANC" => "Cancelled",
            _ => "Unknown"
        };
    }

    private string MapStatusFromSystemB(int code)
    {
        return code switch
        {
            1 => "Pending",
            2 => "Processing",
            3 => "Shipped",
            4 => "Completed",
            5 => "Cancelled",
            _ => "Unknown"
        };
    }

    // Public helpers
    public UnifiedOrder? GetById(string id)
    {
        return _orders.FirstOrDefault(o => string.Equals(o.OrderId, id, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<UnifiedOrder> SearchByStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status)) return _orders;
        var normalized = status.Trim();
        return _orders.Where(o => string.Equals(o.Status, normalized, StringComparison.OrdinalIgnoreCase));
    }
}

