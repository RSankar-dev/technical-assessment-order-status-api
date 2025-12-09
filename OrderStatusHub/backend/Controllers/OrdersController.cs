using Microsoft.AspNetCore.Mvc;
using OrderStatusHub.Services;
using OrderStatusHub.Models;

namespace OrderStatusHub.Controllers;

[ApiController]
[Route("api/order-hub")]
public class OrdersController : ControllerBase
{
    private readonly IOrderIntegrationService _orderIntegrationService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderIntegrationService orderIntegrationService , ILogger<OrdersController> logger)
    {
        _orderIntegrationService = orderIntegrationService;
        _logger = logger;
    }

    /// <summary>
    /// Health endpoint to check API status
    /// </summary>
    /// <returns>Status OK</returns>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "ok", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// GET: /api/orders
    /// </summary>
    /// <returns> All Orders based on validations</returns>    
    [HttpGet("orders")]
    public IActionResult GetAll()
    {
        try
        {
            var orders = _orderIntegrationService.Orders;

            if (orders == null || !orders.Any())
            {
                return NotFound(new
                {
                    message = "No orders found.",
                    code = "ORDERS_EMPTY"
                });
            }

            return Ok(orders); // 200 OK
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders.");
            return StatusCode(500, new
            {
                message = "An unexpected error occurred while fetching orders.",
                code = "SERVER_ERROR"
            });
        }
    }

    /// <summary>
    /// GET: /api/orders/{id}
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("orders/{id}")]
    public IActionResult GetById(string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new
                {
                    message = "Order ID is required.",
                    code = "INVALID_ORDER_ID"
                });
            }

            var order = _orderIntegrationService.GetById(id);

            if (order == null)
            {
                return NotFound(new
                {
                    message = $"Order with ID '{id}' was not found.",
                    code = "ORDER_NOT_FOUND"
                });
            }

            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order {orderId}", id);
            return StatusCode(500, new
            {
                message = "An unexpected error occurred while retrieving the order.",
                code = "SERVER_ERROR"
            });
        }
    }

    /// <summary>
    /// GET: /api/orders/search?status=Processing
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>    
    [HttpGet("orders/search")]
    public IActionResult Search([FromQuery] string? status)
    {
        try
        {
            // Validate: status enum check
            var validStatuses = new[] { "Pending", "Processing", "Shipped", "Completed", "Cancelled" };

            if (!string.IsNullOrWhiteSpace(status) &&
                !validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    message = $"Invalid status '{status}'. Valid values: {string.Join(", ", validStatuses)}",
                    code = "INVALID_STATUS"
                });
            }

            var results = _orderIntegrationService.SearchByStatus(status ?? "");

            if (!results.Any())
            {
                return NotFound(new
                {
                    message = "No orders match the given filter.",
                    code = "NO_MATCHING_ORDERS"
                });
            }

            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching orders with status {status}", status);
            return StatusCode(500, new
            {
                message = "An unexpected error occurred while searching orders.",
                code = "SERVER_ERROR"
            });
        }
    }
}
