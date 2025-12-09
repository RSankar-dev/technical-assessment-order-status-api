using Xunit;
using Moq;
using OrderStatusHub.Controllers;
using OrderStatusHub.Services;
using OrderStatusHub.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace OrderStatusHub.Tests.Controllers;

public class OrdersControllerTests
{
    [Fact]
    public void GetById_ReturnsOk_WhenOrderExists()
    {
        var mockOrderIntegrationService = new Mock<IOrderIntegrationService>();
        mockOrderIntegrationService.Setup(s => s.GetById("A1")).Returns(
            new UnifiedOrder { OrderId = "A1", Status = "Pending" });

        var controller = new OrdersController(mockOrderIntegrationService.Object, new NullLogger<OrdersController>());

        var response = controller.GetById("A1") as OkObjectResult;

        Assert.NotNull(response);
        Assert.Equal(200, response.StatusCode);
    }

    [Fact]
    public void GetById_ReturnsNotFound_WhenOrderDoesNotExist()
    {
        var mockOrderIntegrationService = new Mock<IOrderIntegrationService>();
        mockOrderIntegrationService.Setup(s => s.GetById("B1")).Returns((UnifiedOrder?)null);

        var controller = new OrdersController(mockOrderIntegrationService.Object, new NullLogger<OrdersController>());

        var result = controller.GetById("B1");

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public void Search_ReturnsBadRequest_WhenInvalidStatus()
    {
        var mockOrderIntegrationService = new Mock<IOrderIntegrationService>();
        var controller = new OrdersController(mockOrderIntegrationService.Object, new NullLogger<OrdersController>());

        var result = controller.Search("INVALID_STATUS") as BadRequestObjectResult;

        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
    }
}
