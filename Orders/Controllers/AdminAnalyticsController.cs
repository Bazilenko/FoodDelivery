using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders.Bll.DTOs.Analytics;
using Orders.Shared.DTOs;
using Orders.Bll.Services.Interfaces;
using Orders.Shared.Context;

namespace Orders.Api.Controllers;

[ApiController]
[Route("orders/admin/analytics")]
//[Authorize(Roles = "Admin")]
[Produces("application/json")]
public class AdminAnalyticsController : ControllerBase
{
    private readonly IAdminAnalyticsService _service;
 
    public AdminAnalyticsController(IAdminAnalyticsService service){
         _service = service;
    }
 
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(DashboardDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardDto>> GetPlatformDashboard(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] int topN = 10,
        CancellationToken ct = default)
    {
        var result = await _service.GetPlatformDashboardAsync(new AnalyticsRequest(from, to, topN), ct);
        return Ok(result);
    }
 
    [HttpGet("revenue/summary")]
    [ProducesResponseType(typeof(RevenueSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<RevenueSummaryDto>> GetRevenueSummary(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct = default)
    {
        var result = await _service.GetPlatformRevenueSummaryAsync(new AnalyticsRequest(from, to), ct);
        return Ok(result);
    }
 
    [HttpGet("revenue/by-day")]
    [ProducesResponseType(typeof(IEnumerable<RevenuePeriodDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RevenuePeriodDto>>> GetRevenueByDay(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct = default)
    {
        var result = await _service.GetPlatformRevenueByDayAsync(new AnalyticsRequest(from, to), ct);
        return Ok(result);
    }
 
    [HttpGet("revenue/by-week")]
    [ProducesResponseType(typeof(IEnumerable<RevenuePeriodDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RevenuePeriodDto>>> GetRevenueByWeek(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct = default)
    {
        var result = await _service.GetPlatformRevenueByWeekAsync(new AnalyticsRequest(from, to), ct);
        return Ok(result);
    }
 
    [HttpGet("revenue/by-month")]
    [ProducesResponseType(typeof(IEnumerable<RevenuePeriodDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RevenuePeriodDto>>> GetRevenueByMonth(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct = default)
    {
        var result = await _service.GetPlatformRevenueByMonthAsync(new AnalyticsRequest(from, to), ct);
        return Ok(result);
    }
 
    [HttpGet("orders/summary")]
    [ProducesResponseType(typeof(OrderSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<OrderSummaryDto>> GetOrderSummary(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct = default)
    {
        var result = await _service.GetPlatformOrderSummaryAsync(new AnalyticsRequest(from, to), ct);
        return Ok(result);
    }
 
    [HttpGet("orders/peak-hours")]
    [ProducesResponseType(typeof(IEnumerable<PeakHourDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PeakHourDto>>> GetPeakHours(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct = default)
    {
        var result = await _service.GetPlatformPeakHoursAsync(new AnalyticsRequest(from, to), ct);
        return Ok(result);
    }
 
    [HttpGet("dishes/top-by-quantity")]
    [ProducesResponseType(typeof(IEnumerable<TopDishDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TopDishDto>>> GetTopDishesByQuantity(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] int topN = 10,
        CancellationToken ct = default)
    {
        var result = await _service.GetPlatformTopDishesByQuantityAsync(new AnalyticsRequest(from, to, topN), ct);
        return Ok(result);
    }
 
    [HttpGet("dishes/top-by-revenue")]
    [ProducesResponseType(typeof(IEnumerable<TopDishDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TopDishDto>>> GetTopDishesByRevenue(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] int topN = 10,
        CancellationToken ct = default)
    {
        var result = await _service.GetPlatformTopDishesByRevenueAsync(new AnalyticsRequest(from, to, topN), ct);
        return Ok(result);
    }
 
    [HttpGet("categories/top")]
    [ProducesResponseType(typeof(IEnumerable<TopCategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TopCategoryDto>>> GetTopCategories(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] int topN = 10,
        CancellationToken ct = default)
    {
        var result = await _service.GetPlatformTopCategoriesAsync(new AnalyticsRequest(from, to, topN), ct);
        return Ok(result);
    }
 
    [HttpGet("options/top")]
    [ProducesResponseType(typeof(IEnumerable<TopOptionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TopOptionDto>>> GetTopOptions(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] int topN = 10,
        CancellationToken ct = default)
    {
        var result = await _service.GetPlatformTopOptionsAsync(new AnalyticsRequest(from, to, topN), ct);
        return Ok(result);
    }
 
    [HttpGet("customers/top")]
    [ProducesResponseType(typeof(IEnumerable<TopCustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TopCustomerDto>>> GetTopCustomers(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] int topN = 10,
        CancellationToken ct = default)
    {
        var result = await _service.GetPlatformTopCustomersAsync(new AnalyticsRequest(from, to, topN), ct);
        return Ok(result);
    }
 
    [HttpGet("customers/retention")]
    [ProducesResponseType(typeof(CustomerRetentionDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CustomerRetentionDto>> GetCustomerRetention(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct = default)
    {
        var result = await _service.GetPlatformCustomerRetentionAsync(new AnalyticsRequest(from, to), ct);
        return Ok(result);
    }
 
    [HttpGet("payments/summary")]
    [ProducesResponseType(typeof(PaymentSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaymentSummaryDto>> GetPaymentSummary(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct = default)
    {
        var result = await _service.GetPlatformPaymentSummaryAsync(new AnalyticsRequest(from, to), ct);
        return Ok(result);
    }
}